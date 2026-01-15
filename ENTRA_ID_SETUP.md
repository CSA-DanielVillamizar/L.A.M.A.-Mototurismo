# Guía de Configuración - Autenticación con Microsoft Entra External ID (Azure B2C)

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Configuración del Tenant Azure B2C](#configuración-del-tenant-azure-b2c)
3. [Creación de Aplicación Registrada](#creación-de-aplicación-registrada)
4. [Configuración Segura en appsettings](#configuración-segura-en-appsettings)
5. [Flujo de Autenticación](#flujo-de-autenticación)
6. [Endpoints de API](#endpoints-de-api)
7. [Bypass en DEBUG](#bypass-en-debug)
8. [Implementación Multi-Tenant](#implementación-multi-tenant)
9. [Troubleshooting](#troubleshooting)

---

## Descripción General

Lama.API implementa autenticación moderna con **Microsoft Entra External ID (Azure B2C)** siguiendo Clean Architecture:

- **Tokens JWT**: Emitidos por Azure B2C
- **Sincronización Local**: Claims de Entra se sincronizan a tabla `IdentityUsers`
- **Multi-Tenancy**: Cada usuario aislado por `TenantId`
- **DEBUG Bypass**: Soporte para testing con header `X-Dev-Bypass: true`
- **Vinculación Opcional**: Link entre identidad Entra y miembro LAMA

### Flujo Típico

```
Usuario (navegador)
    ↓ Inicia sesión
Azure B2C (login)
    ↓ Emite JWT con "sub" claim
Cliente (SPA/Postman)
    ↓ Envía: Authorization: Bearer <JWT>
Lama.API (recibe request)
    ↓ TenantResolutionMiddleware: extrae tenant de claims
    ↓ UseAuthentication: valida JWT
    ↓ IdentityUserSyncMiddleware: crea/actualiza IdentityUser
Controlador (autorizado)
```

---

## Configuración del Tenant Azure B2C

### Paso 1: Crear Tenant Azure B2C

1. **Azure Portal** → [Crea nuevo recurso](https://portal.azure.com)
2. Busca: **Azure AD B2C**
3. Haz clic: **Crear**
4. Configura:
   - **Nombre de Organización**: ej: `LAMA Moto`
   - **Nombre de Dominio Inicial**: ej: `lama-moto` (único globalmente)
   - **País/Región**: Selecciona tu país
   - **Suscripción**: Selecciona la tuya
   - **Grupo de Recursos**: Crea o selecciona uno

5. Espera la creación (~10-15 min) ✅

### Paso 2: Navegar al Nuevo Tenant

```
Portal.Azure → Cambiar Directorio
Selecciona: lama-moto.onmicrosoft.com (tu nuevo tenant B2C)
```

---

## Creación de Aplicación Registrada

### Paso 1: Registrar Nueva Aplicación

1. **Azure B2C** → **App registrations**
2. Haz clic: **New registration**
3. Configura:
   - **Name**: `LAMA API`
   - **Supported account types**: `Accounts in this organizational directory only (lama-moto only - Single tenant)`
   - **Redirect URI**: `Web` → `http://localhost:3000` (cambiará en producción)
4. Haz clic: **Register**

**Resultado**: Obtendrás:
- `Application (client) ID`: Guarda este valor (será tu `ClientId`)
- `Directory (tenant) ID`: Necesitarás para construir Authority

### Paso 2: Crear Client Secret (para seguridad server-to-server)

1. **Certificates & secrets**
2. Haz clic: **New client secret**
3. Configura:
   - **Description**: `API Backend Auth`
   - **Expires**: `24 months` (o según política)
4. **Valor**: Copia y guarda **INMEDIATAMENTE** (solo visible una vez)

⚠️ **NUNCA** guardes el secret en `appsettings.json` en producción

### Paso 3: Configurar Permisos de API

1. **API permissions**
2. Haz clic: **Add a permission**
3. Selecciona: **Microsoft APIs** → **Microsoft Graph**
4. Elige: **Delegated permissions**
5. Busca y marca:
   - `openid`
   - `profile`
   - `email`
6. Haz clic: **Add permissions**
7. Haz clic: **Grant admin consent for lama-moto**

---

## Configuración Segura en appsettings

### Paso 1: Archivo appsettings.json (Template - NO SECRETOS)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Lama": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "LamaDb": "Server=(localdb)\\mssqllocaldb;Database=LamaDb;Trusted_Connection=true;"
  },
  "AzureAd": {
    "Authority": "https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin",
    "ClientId": "12345678-1234-1234-1234-123456789012",
    "Audience": "12345678-1234-1234-1234-123456789012",
    "ClientSecret": "CHANGE_ME_IN_USER_SECRETS_PRODUCTION",
    "RedirectUri": "http://localhost:3000",
    "Scopes": ["openid", "profile", "email"],
    "RequireAuthentication": false
  }
}
```

### Paso 2: Usar User Secrets para Secretos (Local Development)

```powershell
# En tu terminal, en el directorio de Lama.API:

# 1. Inicializar User Secrets
dotnet user-secrets init

# 2. Guardar ClientId
dotnet user-secrets set "AzureAd:ClientId" "12345678-1234-1234-1234-123456789012"

# 3. Guardar ClientSecret
dotnet user-secrets set "AzureAd:ClientSecret" "tu-secret-aqui"

# 4. Guardar Authority
dotnet user-secrets set "AzureAd:Authority" "https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin"

# 5. Guardar Audience
dotnet user-secrets set "AzureAd:Audience" "12345678-1234-1234-1234-123456789012"

# Ver todos los secretos guardados:
dotnet user-secrets list
```

### Paso 3: Variables de Entorno en Producción

En **Azure App Service** o tu hosting:

```
ASPNETCORE_ENVIRONMENT = Production
AzureAd__ClientId = <tu-client-id>
AzureAd__ClientSecret = <tu-client-secret>
AzureAd__Authority = https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin
AzureAd__Audience = <tu-client-id>
```

---

## Flujo de Autenticación

### Componentes Involucrados

#### 1. **Program.cs** - Configuración JWT Bearer

```csharp
// Configurar JWT Bearer con Microsoft Identity Web
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    });

// En el middleware pipeline:
app.UseAuthentication();
app.UseAuthorization();

// Sincronizar identidades después de autenticación
app.UseMiddleware<IdentityUserSyncMiddleware>();
```

#### 2. **TenantResolutionMiddleware**

Extrae tenant de:
- Header: `X-Tenant-Id`
- JWT Claim: `tid` (tenant claim)
- Default: `LAMA_DEFAULT`

#### 3. **IdentityUserSyncMiddleware**

En cada request autenticado:
1. Obtiene claims del JWT (`sub`, `email`, `name`)
2. Llama a `IIdentityUserService.EnsureIdentityUserAsync()`
3. Crea o actualiza `IdentityUser` en BD
4. Actualiza `LastLoginAt`

#### 4. **IIdentityUserService** - Abstracción en Application Layer

```csharp
public interface IIdentityUserService
{
    /// Sincroniza claims de Entra con IdentityUser local
    Task<IdentityUser> EnsureIdentityUserAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default);
    
    /// Vincula identidad Entra con miembro LAMA
    Task<IdentityUser> LinkToMemberAsync(string externalSubjectId, int memberId, CancellationToken cancellationToken = default);
    
    /// Obtiene usuario actual desde claims
    Task<IdentityUser> GetCurrentUserAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default);
    
    /// Busca por ID externo (Entra "sub" claim)
    Task<IdentityUser> GetByExternalSubjectIdAsync(string externalSubjectId, CancellationToken cancellationToken = default);
    
    /// Busca por email
    Task<IdentityUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
```

#### 5. **IdentityUserService** - Implementación en Infrastructure

- Operaciones CRUD en tabla `IdentityUsers`
- Respeta `ITenantProvider` para multi-tenancy
- Valida que members existan antes de vincular

---

## Endpoints de API

### POST /api/identity/link

**Vincular identidad Entra con miembro LAMA (Admin)**

```http
POST /api/identity/link HTTP/1.1
Host: localhost:7001
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "externalSubjectId": "sub-from-entra-jwt",
  "memberId": 42
}
```

**Response 200 OK:**

```json
{
  "identityUserId": 5,
  "email": "user@contoso.onmicrosoft.com",
  "displayName": "John Doe",
  "externalSubjectId": "sub-from-entra",
  "memberId": 42,
  "linkedAt": "2025-01-15T10:30:00Z"
}
```

**Errores:**
- `400`: Datos inválidos
- `401`: No autenticado
- `403`: No autorizado (requiere Admin)
- `404`: Miembro no encontrado

### GET /api/identity/me

**Obtener perfil del usuario actual**

```http
GET /api/identity/me HTTP/1.1
Host: localhost:7001
Authorization: Bearer <jwt-token>
```

**Response 200 OK:**

```json
{
  "identityUserId": 5,
  "email": "user@contoso.onmicrosoft.com",
  "displayName": "John Doe",
  "externalSubjectId": "sub-from-entra",
  "memberId": 42,
  "memberName": "Carlos Lopez",
  "isActive": true,
  "createdAt": "2025-01-10T08:00:00Z",
  "lastLoginAt": "2025-01-15T10:30:00Z"
}
```

**Errores:**
- `401`: No autenticado
- `404`: Usuario no tiene registro de identidad

---

## Bypass en DEBUG

### Propósito

En `DEBUG` mode (desarrollo local), permite testing sin configurar un tenant B2C.

### Cómo Funciona

#### AdminController con Bypass

```csharp
#if !DEBUG
    [Authorize(Roles = "Admin")] // En RELEASE: requiere autenticación real
#endif
public async Task<IActionResult> UploadEvidenceAsync(...)
{
#if DEBUG
    // En DEBUG: validar header X-Dev-Bypass: true si no está autenticado
    if (!User?.Identity?.IsAuthenticated == true && !IsValidDevBypass())
        return Unauthorized(new { error = "..." });
#endif
    // Procesar solicitud
}

private bool IsValidDevBypass()
{
#if DEBUG
    return Request.Headers.TryGetValue("X-Dev-Bypass", out var bypassValue) &&
           bypassValue == "true";
#else
    return false; // NUNCA en RELEASE
#endif
}
```

#### Usar en Testing Local

```bash
# Con bypass header (DEBUG mode)
curl -X POST http://localhost:7001/api/admin/evidence/upload \
  -H "X-Dev-Bypass: true" \
  -F "eventId=1" \
  -F "memberId=1" \
  -F "vehicleId=1" \
  -F "evidenceType=START_YEAR" \
  -F "odometerReading=1000" \
  -F "unit=Miles" \
  -F "pilotWithBikePhoto=@photo1.jpg" \
  -F "odometerCloseupPhoto=@photo2.jpg"

# RELEASE mode (producción) - requiere JWT Bearer
curl -X POST http://api.lama.com/api/admin/evidence/upload \
  -H "Authorization: Bearer <jwt-token>" \
  ...
```

### ⚠️ SEGURIDAD

- **DEBUG Bypass**: SOLO funciona cuando `Environment.IsDevelopment() == true`
- **RELEASE**: `X-Dev-Bypass` es ignorado completamente
- **Header solo**: No hay validación de credentials, es testing puro
- **Nunca en producción**: La compilación `RELEASE` elimina todo el bypass

---

## Implementación Multi-Tenant

### Estructura de Datos

```sql
-- Tabla IdentityUsers
Id (PK)                  -- Auto-increment
TenantId (FK, índice)   -- Tenant isolation (GUID)
ExternalSubjectId       -- "sub" claim de Entra (unique per tenant)
Email
DisplayName
MemberId (nullable FK)  -- Vinculación con Members
CreatedAt, LastLoginAt, UpdatedAt
IsActive

-- Índices:
- Unique(TenantId, ExternalSubjectId)
- Index(TenantId, Email)
- Index(TenantId)
- Index(CreatedAt)
```

### Query Filtering Automático

```csharp
// En LamaDbContext.OnModelCreating:
modelBuilder.Entity<IdentityUser>()
    .HasQueryFilter(iu => iu.TenantId == _tenantProvider.CurrentTenantId);
```

Esto significa:
- **Cada query** se filtra automáticamente por tenant actual
- **Imposible leer** datos de otro tenant sin explícitamente deshabilitar el filtro
- **TODAS las operaciones CRUD** respetan el aislamiento

### ITenantProvider

```csharp
public interface ITenantProvider
{
    Guid CurrentTenantId { get; }
    void SetTenantId(Guid tenantId);
}
```

Se resuelve desde:
1. **TenantResolutionMiddleware** → Header `X-Tenant-Id`
2. **JWT Claim** → `tid` (tenant ID claim)
3. **Default** → `LAMA_DEFAULT` GUID

---

## Troubleshooting

### Error: "Invalid token" o "Token validation failed"

**Causas posibles:**
1. Audience no coincide
2. Authority URL incorrecta
3. Token expirado

**Solución:**
```csharp
// Verifica en Program.cs:
options.Authority = "https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin"
options.Audience = "12345678-1234-1234-1234-123456789012" // Debe = ClientId
```

### Error: "Issuer validation failed"

**En DEBUG**, hay una opción para desactivar validación:

```csharp
#if DEBUG
    options.TokenValidationParameters.ValidateIssuer = false;
    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
    options.TokenValidationParameters.ValidateAudience = false;
#endif
```

**⚠️ Nunca en RELEASE**

### Error: "User not found in IdentityUsers"

**Causa**: El usuario no fue sincronizado aún o pertenece a otro tenant

**Solución**:
1. Verificar que `IdentityUserSyncMiddleware` se ejecute
2. Revisar logs de `IdentityUserService`
3. Verificar que `TenantId` sea correcto

### Logs Útiles

```csharp
// Ver en output:
logger.LogInformation("IdentityUser sincronizado para: {Email}", email);
logger.LogError(ex, "Error sincronizando IdentityUser");
logger.LogDebug("Identidad vinculada: {ExternalSubjectId}", id);
```

Activa en `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Lama": "Debug"
  }
}
```

### Postman Testing

1. **Obtener token de Entra**:
   - Usa Postman OAuth 2.0 helper
   - Authority: `https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin`
   - Client ID: Tu app ID
   - Scopes: `openid profile email`

2. **O usar DEBUG bypass** (local):
   ```
   Header: X-Dev-Bypass: true
   ```

3. **Copiar token en Authorization**:
   ```
   Authorization: Bearer <token>
   ```

---

## Resumen

| Aspecto | Valor |
|--------|-------|
| **Proveedor** | Microsoft Entra External ID (B2C) |
| **Autenticación** | JWT Bearer |
| **Base Datos** | SQL Server (tabla IdentityUsers) |
| **Aislamiento** | Multi-tenant por TenantId |
| **Testing Local** | X-Dev-Bypass header (DEBUG) |
| **Vinculación** | Opcional (IdentityUser → Member) |
| **Sincronización** | IdentityUserSyncMiddleware en cada request |
| **Logging** | INFO + DEBUG para troubleshooting |

---

**Documento versión**: 1.0  
**Última actualización**: Enero 2025  
**Mantenido por**: Lama.API Team

