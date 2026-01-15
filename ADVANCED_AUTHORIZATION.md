# Sistema Avanzado de Autorización: Roles + Scopes

## Tabla de Contenidos
1. [Descripción General](#descripción-general)
2. [Arquitectura de Roles](#arquitectura-de-roles)
3. [Arquitectura de Scopes](#arquitectura-de-scopes)
4. [Flujo de Autorización](#flujo-de-autorización)
5. [Implementación en Código](#implementación-en-código)
6. [Ejemplos de Uso](#ejemplos-de-uso)
7. [API de Autorización](#api-de-autorización)
8. [Consideraciones de Seguridad](#consideraciones-de-seguridad)

---

## Descripción General

El sistema de autorización de LAMA implementa un modelo **jerárquico de roles combinado con scopes territoriales**. Este modelo permite:

✅ **Control granular de acceso**: Cada usuario tiene un rol (MEMBER, MTO_CHAPTER, ADMIN_CHAPTER, etc.)  
✅ **Limitación territorial**: Cada usuario tiene scopes que definen el territorio donde puede actuar  
✅ **Auditoría completa**: Cada asignación de rol/scope registra quién la realizó, cuándo y por qué  
✅ **Expiración temporal**: Los roles y scopes pueden expirar automáticamente en una fecha específica  
✅ **Multi-tenancy**: El sistema mantiene aislamiento de datos por tenant  

### Ejemplo Visual

```
Usuario: Juan García
│
├─ Rol: MTO_CHAPTER (puede validar eventos)
│  ├─ AssignedAt: 2025-01-15
│  ├─ ExpiresAt: null (no expira)
│  ├─ AssignedBy: admin@lama.com
│  └─ Reason: "MTO del Capítulo Medellín"
│
└─ Scope: CHAPTER (limitado al capítulo)
   ├─ ScopeId: "Medellín" (ChapterId)
   ├─ AssignedAt: 2025-01-15
   └─ IsActive: true

Resultado: Juan puede validar eventos SOLO en Medellín
```

---

## Arquitectura de Roles

### Definición de Roles

Los roles están ordenados jerárquicamente de menor a mayor privilegio:

```csharp
public enum RoleType
{
    MEMBER                  = 0,  // Miembro básico sin privilegios especiales
    MTO_CHAPTER            = 1,  // Puede validar eventos en su capítulo
    ADMIN_CHAPTER          = 2,  // Administrador a nivel capítulo
    ADMIN_NATIONAL         = 3,  // Administrador a nivel país (múltiples capítulos)
    ADMIN_CONTINENT        = 4,  // Administrador a nivel continente
    ADMIN_INTERNATIONAL    = 5,  // Administrador internacional (múltiples continentes)
    SUPER_ADMIN            = 6   // Acceso total sin restricciones
}
```

### Jerarquía de Privilegios

Cada rol **incluye automáticamente los privilegios del anterior**:

```
MEMBER
  ↓
MTO_CHAPTER (+ puede validar eventos)
  ↓
ADMIN_CHAPTER (+ puede gestionar capítulo)
  ↓
ADMIN_NATIONAL (+ puede gestionar país)
  ↓
ADMIN_CONTINENT (+ puede gestionar continente)
  ↓
ADMIN_INTERNATIONAL (+ puede gestionar Internacional)
  ↓
SUPER_ADMIN (+ acceso total)
```

### Validación de Rol

Para verificar que un usuario tiene un rol específico O superior:

```csharp
// Verificar si es Admin de Capítulo o superior
bool canManageChapter = await authService.HasMinimumRoleAsync(
    externalSubjectId: "user@entra.id",
    minimumRole: RoleType.ADMIN_CHAPTER
);

// Resultado:
// - ADMIN_CHAPTER: true ✅
// - ADMIN_NATIONAL: true ✅ (superior)
// - SUPER_ADMIN: true ✅ (superior)
// - MTO_CHAPTER: false ❌ (inferior)
// - MEMBER: false ❌ (inferior)
```

---

## Arquitectura de Scopes

### Definición de Scopes

Los scopes definen el **territorio geográfico o funcional** donde un usuario puede actuar:

```csharp
public enum ScopeType
{
    CHAPTER    = 0,   // Limitado a un capítulo específico (ChapterId)
    COUNTRY    = 1,   // Limitado a un país (CountryCode: "CO", "AR", etc.)
    CONTINENT  = 2,   // Limitado a un continente (ContinentId)
    GLOBAL     = 3    // Sin limitación geográfica
}
```

### Modelo de Datos

La tabla `UserScopes` almacena el scope de cada usuario:

```sql
UserScopes
├── Id                      (PK)
├── TenantId               (FK)
├── ExternalSubjectId      (FK a JWT "sub" claim)
├── ScopeType              (CHAPTER | COUNTRY | CONTINENT | GLOBAL)
├── ScopeId                (ChapterId|CountryCode|ContinentId|null)
├── IsActive               (bool)
├── ExpiresAt              (datetime, nullable)
├── AssignedAt             (datetime)
├── AssignedBy             (ExternalSubjectId del admin)
├── Reason                 (auditoría)
└── UpdatedAt              (datetime)
```

### Ejemplos de Configuración

| Usuario | Rol | ScopeType | ScopeId | Acceso |
|---------|-----|-----------|---------|--------|
| Juan | MTO_CHAPTER | CHAPTER | "Medellín" | Eventos en Medellín |
| María | ADMIN_CHAPTER | CHAPTER | "Bogotá" | Gestionar capítulo Bogotá |
| Carlos | ADMIN_NATIONAL | COUNTRY | "CO" | Todos los capítulos de Colombia |
| Ana | ADMIN_CONTINENT | CONTINENT | "SouthAmerica" | Todos los países de Sudamérica |
| Roberto | ADMIN_INTERNATIONAL | GLOBAL | null | Sin restricciones geográficas |
| Director | SUPER_ADMIN | GLOBAL | null | Acceso total (sin validación de scope) |

---

## Flujo de Autorización

### Flujo 1: Validar Acceso a un Endpoint

```
Usuario hace request con JWT
│
├─ API extrae ExternalSubjectId del claim "sub"
│
├─ Middleware de autorización verifica:
│  ├─ ¿Usuario autenticado?
│  ├─ ¿Tiene rol mínimo requerido?
│  └─ ¿Su scope cubre el recurso?
│
├─ ScopeAuthorizationHandler ejecuta validación
│  ├─ Consulta UserRoles (filtradas por TenantId actual)
│  ├─ Consulta UserScopes
│  ├─ Valida expiración (ExpiresAt > DateTime.UtcNow)
│  └─ Valida IsActive = true
│
└─ Resultado:
   ├─ ✅ Autorización exitosa → continúa
   └─ ❌ Acceso denegado → HTTP 403 Forbidden
```

### Flujo 2: Asignar un Rol

```csharp
await authService.AssignRoleAsync(
    externalSubjectId: "user@example.com",
    role: RoleType.ADMIN_CHAPTER,
    assignedByExternalSubjectId: "admin@example.com",
    reason: "Promovido a Admin del Capítulo Medellín",
    expiresAt: DateTime.UtcNow.AddYears(1)
);
```

**Qué ocurre internamente:**
1. Desactiva rol anterior del mismo tipo (si existe)
2. Crea nuevo registro en UserRoles
3. Registra auditoría: quién, cuándo, por qué
4. Retorna la entidad creada

### Flujo 3: Verificar Acceso a Recurso Específico

```csharp
bool hasAccess = await authService.CanAccessResourceAsync(
    externalSubjectId: "user@example.com",
    requiredRole: RoleType.ADMIN_CHAPTER,
    requiredScopeType: ScopeType.CHAPTER,
    resourceScopeId: "Medellín"  // ChapterId del evento
);
```

**Validación:**
1. ¿Usuario tiene rol >= ADMIN_CHAPTER? → Sí/No
2. ¿Usuario es SUPER_ADMIN? → Acceso automático
3. ¿Usuario tiene scope GLOBAL? → Acceso automático
4. ¿Usuario tiene scope CHAPTER con ScopeId = "Medellín"? → Acceso

---

## Implementación en Código

### 1. Entidades de Dominio

#### UserRole
Archivo: [src/Lama.Domain/Entities/UserRole.cs](src/Lama.Domain/Entities/UserRole.cs)

```csharp
public class UserRole
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public string ExternalSubjectId { get; set; }  // "sub" claim del JWT
    public RoleType Role { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }  // null = no expira
    public bool IsActive { get; set; }
    public string? Reason { get; set; }  // Para auditoría
    public string AssignedBy { get; set; }  // ExternalSubjectId del admin
    public DateTime UpdatedAt { get; set; }
}

public enum RoleType
{
    MEMBER = 0,
    MTO_CHAPTER = 1,
    ADMIN_CHAPTER = 2,
    ADMIN_NATIONAL = 3,
    ADMIN_CONTINENT = 4,
    ADMIN_INTERNATIONAL = 5,
    SUPER_ADMIN = 6
}
```

#### UserScope
Archivo: [src/Lama.Domain/Entities/UserScope.cs](src/Lama.Domain/Entities/UserScope.cs)

```csharp
public class UserScope
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public string ExternalSubjectId { get; set; }
    public ScopeType ScopeType { get; set; }
    public string? ScopeId { get; set; }  // ChapterId, CountryCode, ContinentId, o null
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string? Reason { get; set; }
    public string AssignedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum ScopeType
{
    CHAPTER = 0,
    COUNTRY = 1,
    CONTINENT = 2,
    GLOBAL = 3
}
```

### 2. Interfaz de Servicio

Archivo: [src/Lama.Application/Services/IUserAuthorizationService.cs](src/Lama.Application/Services/IUserAuthorizationService.cs)

**Métodos principales:**

```csharp
// Consultas
Task<IEnumerable<UserRole>> GetUserRolesAsync(string externalSubjectId);
Task<IEnumerable<UserScope>> GetUserScopesAsync(string externalSubjectId);
Task<bool> HasRoleAsync(string externalSubjectId, RoleType role);
Task<bool> HasMinimumRoleAsync(string externalSubjectId, RoleType minimumRole);
Task<bool> HasScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId);
Task<bool> CanAccessResourceAsync(string externalSubjectId, RoleType requiredRole, 
                                    ScopeType requiredScopeType, string? resourceScopeId);

// Comandos
Task<UserRole> AssignRoleAsync(string externalSubjectId, RoleType role, 
                               string assignedByExternalSubjectId, string reason, 
                               DateTime? expiresAt = null);
Task<UserScope> AssignScopeAsync(string externalSubjectId, ScopeType scopeType, 
                                 string? scopeId, string assignedByExternalSubjectId, 
                                 string reason, DateTime? expiresAt = null);
Task<bool> RevokeRoleAsync(string externalSubjectId, RoleType role);
Task<bool> RevokeScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId);
```

### 3. Implementación de Servicio

Archivo: [src/Lama.Infrastructure/Services/UserAuthorizationService.cs](src/Lama.Infrastructure/Services/UserAuthorizationService.cs)

- **330+ líneas** con logging completo
- Respeta multi-tenancy automáticamente
- Valida expiración (ExpiresAt > DateTime.UtcNow)
- Deactiva roles anteriores al asignar nuevos
- Registra auditoría en cada operación

### 4. ClaimsHelper

Archivo: [src/Lama.API/Utilities/ClaimsHelper.cs](src/Lama.API/Utilities/ClaimsHelper.cs)

Extrae información de JWT tokens:

```csharp
// Obtener ExternalSubjectId del claim "sub"
string userId = ClaimsHelper.GetExternalSubjectId(User);

// Obtener TenantId del claim "tid"
Guid tenantId = ClaimsHelper.GetTenantIdFromClaims(User);

// Try-catch versions seguras
bool success = ClaimsHelper.TryGetExternalSubjectId(User, out var userId);
```

### 5. AuthorizationHandler Personalizado

Archivo: [src/Lama.API/Authorization/ScopeAuthorizationHandler.cs](src/Lama.API/Authorization/ScopeAuthorizationHandler.cs)

Valida automáticamente en endpoints:

```csharp
protected override async Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    ResourceAuthorizationRequirement requirement)
{
    // Extrae ExternalSubjectId
    // Consulta UserRoles y UserScopes
    // Valida rol mínimo requerido
    // Llama a context.Succeed(requirement) si autorizado
}
```

### 6. Políticas de Autorización

Archivo: [src/Lama.API/Program.cs](src/Lama.API/Program.cs)

```csharp
builder.Services.AddAuthorization(options =>
{
    // MTO puede validar eventos en su capítulo
    options.AddPolicy("CanValidateEvent", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.MTO_CHAPTER, 
            ScopeType.CHAPTER)));

    // Admin de capítulo puede gestionar capítulo
    options.AddPolicy("CanManageChapter", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.ADMIN_CHAPTER,
            ScopeType.CHAPTER)));

    // Admin nacional puede gestionar país
    options.AddPolicy("CanManageCountry", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.ADMIN_NATIONAL,
            ScopeType.COUNTRY)));

    // Admin continental puede gestionar continente
    options.AddPolicy("CanManageContinent", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.ADMIN_CONTINENT,
            ScopeType.CONTINENT)));

    // Admin internacional puede gestionar global
    options.AddPolicy("CanManageGlobal", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.ADMIN_INTERNATIONAL,
            ScopeType.GLOBAL)));

    // Solo SUPER_ADMIN
    options.AddPolicy("IsSuperAdmin", policy =>
        policy.Requirements.Add(new ResourceAuthorizationRequirement(
            RoleType.SUPER_ADMIN,
            ScopeType.GLOBAL)));
});
```

---

## Ejemplos de Uso

### Ejemplo 1: Aplicar Política en Endpoint

```csharp
[HttpPost("events/{eventId}/attendees/validate")]
[Authorize(Policy = "CanValidateEvent")]  // MTO_CHAPTER + CHAPTER scope requerido
public async Task<IActionResult> ValidateAttendanceAsync(
    int eventId,
    [FromBody] AttendanceValidationRequest request)
{
    // ClaimsHelper extrae automáticamente ExternalSubjectId
    var userId = ClaimsHelper.GetExternalSubjectId(User);
    
    // El handler verificó que tiene rol MTO_CHAPTER mínimo
    // La aplicación puede hacer validaciones adicionales de scope si es necesario
    
    // Obtener el evento para verificar su ChapterId
    var @event = await _eventRepository.GetByIdAsync(eventId);
    
    // Verificar que el usuario tiene scope en el capítulo del evento
    bool canAccess = await _authService.CanAccessResourceAsync(
        userId,
        RoleType.MTO_CHAPTER,
        ScopeType.CHAPTER,
        @event.ChapterId.ToString());
    
    if (!canAccess)
        return Forbid("No tienes acceso al capítulo de este evento");
    
    // Proceder con validación...
}
```

### Ejemplo 2: Asignar un Rol en Endpoint Admin

```csharp
[HttpPost("admin/users/{userId}/roles")]
[Authorize(Policy = "CanManageChapter")]  // Admin de capítulo mínimo
public async Task<IActionResult> AssignRoleAsync(
    string userId,
    [FromBody] AssignRoleRequest request)
{
    var adminId = ClaimsHelper.GetExternalSubjectId(User);
    
    var userRole = await _authService.AssignRoleAsync(
        externalSubjectId: userId,
        role: request.Role,
        assignedByExternalSubjectId: adminId,
        reason: request.Reason,
        expiresAt: request.ExpiresAt);
    
    return Ok(userRole);
}

public class AssignRoleRequest
{
    public RoleType Role { get; set; }
    public string Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }  // null = no expira
}
```

### Ejemplo 3: Consultar Roles de Usuario

```csharp
[HttpGet("admin/users/{userId}/roles")]
[Authorize(Policy = "CanManageChapter")]
public async Task<IActionResult> GetUserRolesAsync(string userId)
{
    var roles = await _authService.GetUserRolesAsync(userId);
    
    return Ok(new
    {
        userId,
        roles = roles.Select(r => new
        {
            r.Role,
            r.AssignedAt,
            r.ExpiresAt,
            r.IsActive,
            r.Reason,
            r.AssignedBy
        })
    });
}
```

### Ejemplo 4: Revocar un Rol

```csharp
[HttpDelete("admin/users/{userId}/roles/{role}")]
[Authorize(Policy = "CanManageChapter")]
public async Task<IActionResult> RevokeRoleAsync(string userId, RoleType role)
{
    bool revoked = await _authService.RevokeRoleAsync(userId, role);
    
    if (!revoked)
        return NotFound($"El usuario no tiene el rol {role}");
    
    return NoContent();
}
```

---

## API de Autorización

### IUserAuthorizationService - Referencia Completa

#### GetUserRolesAsync
```csharp
/// <summary>
/// Obtiene todos los roles activos asignados a un usuario.
/// </summary>
Task<IEnumerable<UserRole>> GetUserRolesAsync(
    string externalSubjectId, 
    CancellationToken cancellationToken = default);

// Ejemplo:
var roles = await authService.GetUserRolesAsync("user@example.com");
foreach (var role in roles)
{
    Console.WriteLine($"Rol: {role.Role}, Expira: {role.ExpiresAt}");
}
```

#### HasMinimumRoleAsync
```csharp
/// <summary>
/// Verifica si un usuario tiene un rol igual o superior.
/// </summary>
Task<bool> HasMinimumRoleAsync(
    string externalSubjectId,
    RoleType minimumRole,
    CancellationToken cancellationToken = default);

// Ejemplo:
bool canManage = await authService.HasMinimumRoleAsync(
    "user@example.com",
    RoleType.ADMIN_CHAPTER);
// true si es ADMIN_CHAPTER o superior (ADMIN_NATIONAL, ADMIN_CONTINENT, etc.)
```

#### CanAccessResourceAsync
```csharp
/// <summary>
/// Verifica si un usuario tiene acceso a un recurso específico.
/// </summary>
Task<bool> CanAccessResourceAsync(
    string externalSubjectId,
    RoleType requiredRole,
    ScopeType requiredScopeType,
    string? resourceScopeId,
    CancellationToken cancellationToken = default);

// Ejemplo:
bool canAccess = await authService.CanAccessResourceAsync(
    "user@example.com",
    RoleType.ADMIN_CHAPTER,
    ScopeType.CHAPTER,
    "5");  // ChapterId del recurso

// Retorna true si:
// - Usuario es SUPER_ADMIN, O
// - Usuario es ADMIN_CHAPTER+ Y tiene scope GLOBAL O scope CHAPTER:5
```

#### AssignRoleAsync
```csharp
/// <summary>
/// Asigna un rol a un usuario con auditoría.
/// </summary>
Task<UserRole> AssignRoleAsync(
    string externalSubjectId,
    RoleType role,
    string assignedByExternalSubjectId,
    string reason,
    DateTime? expiresAt = null,
    CancellationToken cancellationToken = default);

// Ejemplo:
var userRole = await authService.AssignRoleAsync(
    externalSubjectId: "user@example.com",
    role: RoleType.MTO_CHAPTER,
    assignedByExternalSubjectId: "admin@example.com",
    reason: "Promovido a MTO del Capítulo Medellín",
    expiresAt: DateTime.UtcNow.AddYears(1));

Console.WriteLine($"Rol asignado: {userRole.Role}, ID: {userRole.Id}");
```

#### AssignScopeAsync
```csharp
/// <summary>
/// Asigna un scope (territorio) a un usuario.
/// </summary>
Task<UserScope> AssignScopeAsync(
    string externalSubjectId,
    ScopeType scopeType,
    string? scopeId,
    string assignedByExternalSubjectId,
    string reason,
    DateTime? expiresAt = null,
    CancellationToken cancellationToken = default);

// Ejemplo - Capítulo:
var scope = await authService.AssignScopeAsync(
    externalSubjectId: "user@example.com",
    scopeType: ScopeType.CHAPTER,
    scopeId: "5",  // ChapterId
    assignedByExternalSubjectId: "admin@example.com",
    reason: "Asignado al Capítulo Medellín");

// Ejemplo - País:
var scope = await authService.AssignScopeAsync(
    externalSubjectId: "user@example.com",
    scopeType: ScopeType.COUNTRY,
    scopeId: "CO",  // CountryCode
    assignedByExternalSubjectId: "admin@example.com",
    reason: "Asignado a Colombia");

// Ejemplo - Global:
var scope = await authService.AssignScopeAsync(
    externalSubjectId: "user@example.com",
    scopeType: ScopeType.GLOBAL,
    scopeId: null,  // Sin restriction
    assignedByExternalSubjectId: "admin@example.com",
    reason: "Acceso global");
```

---

## Consideraciones de Seguridad

### 1. Multi-Tenancy Aislamiento
- Todos los queries aplican automáticamente el filtro `TenantId == CurrentTenantId`
- Un usuario de TENANT_A nunca puede ver/acceder a datos de TENANT_B
- La solución: `HasQueryFilter` en DbContext

### 2. Validación de Expiración
- Los roles/scopes con `ExpiresAt <= DateTime.UtcNow` son automáticamente ignorados
- No requiere job background (validación en cada query)
- Sistema de "soft-delete" manteniendo auditoría

### 3. Auditoría Completa
- Cada asignación de rol/scope registra:
  - ✅ Quién asignó (`AssignedBy`)
  - ✅ Cuándo (`AssignedAt`)
  - ✅ Por qué (`Reason`)
  - ✅ Última actualización (`UpdatedAt`)

### 4. Jerarquía de Roles
- La validación `HasMinimumRoleAsync` usa comparación numérica
- SUPER_ADMIN (6) >= ADMIN_CHAPTER (2) = true
- Imposible saltar niveles

### 5. SUPER_ADMIN Bypass
- Usuarios con rol SUPER_ADMIN obtienen acceso automático
- No necesitan scope específico (cualquier recurso)
- Útil para administradores de plataforma

### 6. Validación de JWT
- El `ExternalSubjectId` viene del claim "sub" del JWT
- Validación de JWT realizada por middleware `JwtBearerDefaults.AuthenticationScheme`
- ClaimsHelper valida que el claim existe antes de usar

### 7. Deactivación vs Eliminación
- Los roles/scopes se "desactivan" (`IsActive = false`), no se eliminan
- Mantiene historial completo para auditoría
- Permite reactivación si es necesario

---

## Troubleshooting

### Problema: "Usuario no autenticado"
```
Error: ClaimsHelper.GetExternalSubjectId() → InvalidOperationException
```
**Solución:**
- Verificar que el JWT contiene claim "sub"
- En DEBUG mode, la validación de JWT es más permisiva
- Ver configuración de AzureAd en appsettings.json

### Problema: "Acceso denegado 403"
```
ScopeAuthorizationHandler → context.Fail()
```
**Posibles causas:**
1. Usuario no tiene rol mínimo requerido
2. Rol expiró (ExpiresAt < DateTime.UtcNow)
3. Rol está desactivado (IsActive = false)

**Debug:**
```csharp
var roles = await authService.GetUserRolesAsync(userId);
var activeRoles = roles.Where(r => r.IsActive && (r.ExpiresAt == null || r.ExpiresAt > DateTime.UtcNow));
```

### Problema: "No se asignó el scope correctamente"
```
Usuario tiene rol pero CanAccessResourceAsync retorna false
```
**Verificación:**
```csharp
var scopes = await authService.GetUserScopesAsync(userId);
foreach (var scope in scopes)
{
    Console.WriteLine($"Scope: {scope.ScopeType}:{scope.ScopeId}, Active: {scope.IsActive}");
}
```

---

## Próximos Pasos

### 1. Aplicar Políticas a Endpoints Existentes
- [ ] EventController: `[Authorize(Policy="CanValidateEvent")]`
- [ ] AdminController: `[Authorize(Policy="CanManageChapter")]`
- [ ] ChapterController: `[Authorize(Policy="CanManageChapter")]`

### 2. Crear Endpoints de Administración
- [ ] POST `/api/admin/users/{userId}/roles` - Asignar rol
- [ ] DELETE `/api/admin/users/{userId}/roles/{role}` - Revocar rol
- [ ] POST `/api/admin/users/{userId}/scopes` - Asignar scope
- [ ] GET `/api/admin/users/{userId}/roles` - Listar roles
- [ ] GET `/api/admin/users/{userId}/scopes` - Listar scopes

### 3. Tests Unitarios
- [ ] UserAuthorizationService.HasMinimumRoleAsync
- [ ] UserAuthorizationService.CanAccessResourceAsync
- [ ] ScopeAuthorizationHandler
- [ ] ClaimsHelper.GetExternalSubjectId

### 4. Documentación de Usuario Final
- [ ] Matriz de roles y permisos por área
- [ ] Procedimientos de asignación de roles
- [ ] FAQ sobre acceso denegado

---

## Referencias

**Archivos clave del sistema:**
- Entities: [UserRole.cs](src/Lama.Domain/Entities/UserRole.cs), [UserScope.cs](src/Lama.Domain/Entities/UserScope.cs)
- Service Interface: [IUserAuthorizationService.cs](src/Lama.Application/Services/IUserAuthorizationService.cs)
- Service Implementation: [UserAuthorizationService.cs](src/Lama.Infrastructure/Services/UserAuthorizationService.cs)
- Handler: [ScopeAuthorizationHandler.cs](src/Lama.API/Authorization/ScopeAuthorizationHandler.cs)
- Utilities: [ClaimsHelper.cs](src/Lama.API/Utilities/ClaimsHelper.cs)
- Configuration: [Program.cs](src/Lama.API/Program.cs)
- Migrations: [20260115_AddUserRoleAndUserScope.cs](src/Lama.Infrastructure/Migrations/20260115_AddUserRoleAndUserScope.cs)

**Documentación relacionada:**
- [ENTRA_ID_SETUP.md](ENTRA_ID_SETUP.md) - Autenticación JWT
- [Clean Architecture](https://learn.microsoft.com/es-es/dotnet/architecture/clean-code-dotnet)

---

**Última actualización:** 2025-01-15  
**Versión:** 1.0  
**Estado:** Implementado y compilado exitosamente ✅
