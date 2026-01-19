# ARQUITECTURA DE AUTENTICACIÓN Y AUTORIZACIÓN

## Overview

Sistema de autenticación Production-Ready que combina:
- **Microsoft Entra External ID** (B2C) como Identity Provider
- **Refresh Tokens Rotativos** en cookies httpOnly con detección de reuso
- **Access Tokens JWT** cortos (15 min) emitidos por la aplicación
- **Autorización Jerárquica** basada en roles + scopes

---

## Flujo de Autenticación: Exchange Flow

```
┌─────────┐                ┌──────────────┐              ┌─────────────┐
│ Browser │                │   Frontend   │              │   Backend   │
│         │                │  (Next.js)   │              │  (ASP.NET)  │
└────┬────┘                └──────┬───────┘              └──────┬──────┘
     │                            │                             │
     │  1. Redirect to Entra ID   │                             │
     ├───────────────────────────>│                             │
     │                            │                             │
     │  2. User authenticates     │                             │
     │     with Entra ID          │                             │
     ├───────────────────────────>│                             │
     │                            │                             │
     │  3. Entra returns          │                             │
     │     id_token/access_token  │                             │
     │<───────────────────────────┤                             │
     │                            │                             │
     │                            │  4. POST /api/auth/exchange │
     │                            │     { idToken: "..." }      │
     │                            ├────────────────────────────>│
     │                            │                             │
     │                            │  5. Validate Entra token    │
     │                            │     & sync IdentityUser     │
     │                            │<────────────────────────────┤
     │                            │                             │
     │                            │  6. Return app access token │
     │                            │     + Set refresh_token     │
     │                            │     cookie (httpOnly)       │
     │                            │<────────────────────────────┤
     │                            │                             │
     │  7. Store access token     │                             │
     │     in memory (React state)│                             │
     │<───────────────────────────┤                             │
     │                            │                             │
     │  8. API calls with         │                             │
     │     Authorization: Bearer  │                             │
     │     <app_access_token>     │                             │
     │                            ├────────────────────────────>│
     │                            │                             │
```

### Paso a Paso

1. **Frontend redirige a Entra ID** para login
2. **Usuario se autentica** con Entra ID (email/password, Google, GitHub)
3. **Entra ID devuelve** `id_token` o `access_token`
4. **Frontend llama** a `POST /api/auth/exchange` con el token de Entra
5. **Backend valida** el token de Entra y:
   - Sincroniza usuario en tabla `IdentityUsers`
   - Carga roles y scopes del usuario
   - Genera **access token de la app** (JWT interno, 15 min)
   - Genera **refresh token rotativo** (hash almacenado en DB, 7 días)
   - Setea refresh token en cookie httpOnly/Secure
6. **Frontend recibe** access token en response body
7. **Frontend almacena** access token en memoria (NO localStorage)
8. **Frontend incluye** access token en header `Authorization: Bearer <token>` en cada API call
9. **Backend valida** access token interno (no Entra) y autoriza según roles/scopes

---

## Refresh Token Rotation

### Características

- **Rotativo**: Cada refresh invalida el anterior y genera nuevo
- **Detección de reuso**: Si se reusa un token revocado, **se revoca toda la cadena**
- **httpOnly cookie**: No accesible desde JavaScript (XSS protection)
- **Secure flag**: Solo HTTPS en producción
- **SameSite=Strict**: CSRF protection (cambiar a Lax si cross-site)
- **Hash SHA-256 con HMAC**: Token nunca se almacena en claro

### Flujo de Refresh

```
┌─────────┐              ┌──────────────┐              ┌─────────────┐
│ Browser │              │   Frontend   │              │   Backend   │
└────┬────┘              └──────┬───────┘              └──────┬──────┘
     │                          │                             │
     │  Access token expired    │                             │
     │  (401 response)          │                             │
     │<─────────────────────────┤                             │
     │                          │                             │
     │                          │  POST /api/auth/refresh-    │
     │                          │  session (cookie auto-sent) │
     │                          ├────────────────────────────>│
     │                          │                             │
     │                          │  Validate refresh token     │
     │                          │  Rotate: revoke old +       │
     │                          │  issue new                  │
     │                          │<────────────────────────────┤
     │                          │                             │
     │                          │  New access token +         │
     │                          │  New refresh cookie         │
     │                          │<────────────────────────────┤
     │                          │                             │
     │  Update access token     │                             │
     │<─────────────────────────┤                             │
     │                          │                             │
     │  Retry original request  │                             │
     │                          ├────────────────────────────>│
     │                          │                             │
```

### Detección de Reuso

Si un atacante roba un refresh token **ya usado** (revocado):

1. Backend detecta que el token está revocado pero se intenta usar
2. Backend identifica la cadena de tokens (vía `ReplacedByTokenId`)
3. **Revoca TODA la cadena** (seguridad proactiva)
4. Usuario legítimo debe re-autenticarse con Entra ID

---

## Autorización: Roles vs Scopes

### Roles (claims: `roles`, `ClaimTypes.Role`)

- **Jerarquía organizacional**: SUPER_ADMIN → ADMIN_INTERNATIONAL → ADMIN_CONTINENT → ADMIN_NATIONAL → ADMIN_CHAPTER → MTO_CHAPTER → SECRETARY_CHAPTER → MEMBER
- **Uso**: Determinar **QUÉ** puede hacer un usuario en el backoffice
- **Ejemplo**: Solo MTO_CHAPTER o superior puede validar eventos de su capítulo

### Scopes (claims: `scp`)

- **Permisos de API**: `api.read`, `api.write`, `ranking.read`, `evidence.upload`, etc.
- **Uso**: Limitar **CÓMO** interactúa un cliente (frontend, mobile app, third-party)
- **Ejemplo**: Una app móvil solo con `ranking.read` no puede subir evidencias

### ScopeType (ámbito territorial)

- **CHAPTER**: Usuario gestiona solo su capítulo
- **COUNTRY**: Usuario gestiona todos los capítulos de su país
- **CONTINENT**: Usuario gestiona todos los países de su continente
- **GLOBAL**: Usuario gestiona todo (internacional)

### Policies en ASP.NET Core

Definidas en [Program.cs](src/Lama.API/Program.cs):

```csharp
// Ejemplo: Validar eventos requiere ser MTO_CHAPTER del capítulo o superior
options.AddPolicy("CanValidateEvent", policy =>
    policy.Requirements.Add(new ResourceAuthorizationRequirement(
        RoleType.MTO_CHAPTER, 
        ScopeType.CHAPTER)));
```

Uso en controllers:

```csharp
[HttpPost("validate/{eventId}")]
[Authorize(Policy = "CanValidateEvent")]
public async Task<IActionResult> ValidateEvent(int eventId) { ... }
```

Handler personalizado: [ScopeAuthorizationHandler.cs](src/Lama.API/Authorization/ScopeAuthorizationHandler.cs)

---

## Access Token (App JWT)

### Claims incluidos

```json
{
  "sub": "123",                      // IdentityUserId
  "email": "user@example.com",       
  "name": "John Doe",
  "tenant_id": "00000000-...",
  "member_id": "456",                // Si está vinculado a Member
  "roles": ["MTO_CHAPTER", "MEMBER"],
  "scp": ["api.read", "api.write", "ranking.read"],
  "iss": "lama-app",
  "aud": "lama-api",
  "exp": 1234567890,
  "iat": 1234567800
}
```

### Configuración (appsettings.json)

```json
{
  "Jwt": {
    "SecretKey": "CHANGE_ME_IN_PRODUCTION_32_CHARS_MIN",
    "Issuer": "lama-app",
    "Audience": "lama-api",
    "AccessTokenLifetimeMinutes": "15",
    "RefreshTokenLifetimeDays": "7"
  }
}
```

**IMPORTANTE**: En producción, usar **User Secrets** o **Azure Key Vault** para `SecretKey`.

---

## CORS y Cookies

### Configuración (Program.cs)

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:3002";
        policy.WithOrigins(frontendUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // CRÍTICO para cookies
    });
});
```

### Cookies httpOnly

```csharp
var cookieOptions = new CookieOptions
{
    HttpOnly = true,                // No accesible desde JS
    Secure = true,                  // Solo HTTPS (false en dev)
    SameSite = SameSiteMode.Strict, // CSRF protection
    Expires = refreshExpiresAt,     // 7 días
    Path = "/",
    IsEssential = true
};
```

**IMPORTANTE**: Si frontend está en otro dominio (cross-site), cambiar `SameSite` a `Lax` o `None` (requiere HTTPS).

### CSRF Protection

- **SameSite=Strict**: Suficiente para same-site apps
- **SameSite=None**: Requiere header anti-CSRF adicional (doble submit pattern)

---

## Endpoints de Autenticación

### `POST /api/v1/auth/exchange`

Intercambia token de Entra ID por sesión de app.

**Request:**
```json
{
  "idToken": "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdl..."
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 900,
  "user": {
    "id": 123,
    "email": "user@example.com",
    "displayName": "John Doe",
    "memberId": 456,
    "memberName": "John Doe",
    "chapterName": "Bogotá",
    "roles": ["MTO_CHAPTER", "MEMBER"],
    "scopes": ["api.read", "api.write"],
    "tenantId": "00000000-0000-0000-0000-000000000001"
  }
}
```

**Cookies (Set-Cookie):**
```
refresh_token=<base64_token>; HttpOnly; Secure; SameSite=Strict; Path=/; Expires=...
```

**Rate Limit:** 10 req/min por IP

---

### `POST /api/v1/auth/refresh-session`

Refresca access token usando refresh token rotativo en cookie.

**Request:** *(cookie auto-enviada)*

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 900,
  "user": { ... }
}
```

**Cookies (Set-Cookie):**
```
refresh_token=<new_token>; HttpOnly; Secure; ...
```

**Error (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Refresh token expired"
}
```

---

### `POST /api/v1/auth/logout-session`

Revoca refresh token y limpia cookie.

**Request:** *(cookie auto-enviada)*

**Response (200 OK):**
```json
{
  "message": "Logged out successfully"
}
```

**Cookies (Set-Cookie):**
```
refresh_token=; HttpOnly; Secure; ...; Max-Age=0
```

---

### `GET /api/v1/auth/me`

Obtiene información del usuario autenticado.

**Headers:**
```
Authorization: Bearer <app_access_token>
```

**Response (200 OK):**
```json
{
  "id": 123,
  "email": "user@example.com",
  "displayName": "John Doe",
  "memberId": 456,
  "memberName": "John Doe",
  "chapterName": "Bogotá",
  "roles": ["MTO_CHAPTER", "MEMBER"],
  "scopes": ["api.read", "api.write"],
  "tenantId": "00000000-0000-0000-0000-000000000001"
}
```

---

## Esquema de Base de Datos

### RefreshTokens

```sql
CREATE TABLE RefreshTokens (
    Id INT PRIMARY KEY IDENTITY,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    IdentityUserId INT NOT NULL,
    TokenHash NVARCHAR(256) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    ReplacedByTokenId INT NULL,
    RevocationReason NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedByIp NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    
    CONSTRAINT FK_RefreshTokens_IdentityUsers 
        FOREIGN KEY (IdentityUserId) 
        REFERENCES IdentityUsers(Id) ON DELETE CASCADE,
    
    INDEX IX_RefreshTokens_TokenHash (TokenHash),
    INDEX IX_RefreshTokens_IdentityUserId (IdentityUserId),
    INDEX IX_RefreshTokens_ExpiresAt (ExpiresAt)
);
```

**Generar migration:**
```powershell
cd src/Lama.Infrastructure
dotnet ef migrations add AddRefreshTokens --project ../Lama.Infrastructure --startup-project ../Lama.API
dotnet ef database update --project ../Lama.Infrastructure --startup-project ../Lama.API
```

---

## Seguridad: Best Practices

### ✅ Implementado

- ✅ Refresh tokens rotativos con detección de reuso
- ✅ Tokens hasheados (SHA-256 HMAC) en DB
- ✅ Cookies httpOnly/Secure
- ✅ Access tokens cortos (15 min)
- ✅ SameSite=Strict (CSRF protection)
- ✅ Rate limiting en endpoints de auth
- ✅ Logging estructurado de eventos de seguridad
- ✅ Validación estricta de tokens de Entra ID
- ✅ Middleware de sincronización IdentityUser

### 🔄 Pendiente

- 🔄 Implementar CSP (Content Security Policy) en frontend
- 🔄 Azure Key Vault para secrets en producción
- 🔄 Alerting en detección de reuso (Azure Monitor)
- 🔄 IP whitelisting opcional para admins
- 🔄 MFA para roles críticos (SUPER_ADMIN, etc.)

---

## Testing

### Probar Exchange Flow

```bash
# 1. Obtener token de Entra ID (simular o usar real)
# 2. Exchange token
curl -X POST http://localhost:5000/api/v1/auth/exchange \
  -H "Content-Type: application/json" \
  -d '{"idToken": "eyJ0eXAi..."}'

# 3. Guardar access_token de response y refresh_token de cookie
# 4. Llamar API con access token
curl -X GET http://localhost:5000/api/v1/auth/me \
  -H "Authorization: Bearer <access_token>"

# 5. Refrescar token
curl -X POST http://localhost:5000/api/v1/auth/refresh-session \
  --cookie "refresh_token=<token>" \
  -c cookies.txt

# 6. Logout
curl -X POST http://localhost:5000/api/v1/auth/logout-session \
  --cookie "refresh_token=<token>"
```

---

## Referencias

- [Microsoft Entra External ID Docs](https://learn.microsoft.com/en-us/entra/external-id/)
- [OWASP Cheat Sheet: Authentication](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [RFC 6749: OAuth 2.0](https://datatracker.ietf.org/doc/html/rfc6749)
- [RFC 7519: JSON Web Token (JWT)](https://datatracker.ietf.org/doc/html/rfc7519)
