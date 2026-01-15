# Development Authentication Bypass

## 🎯 Propósito

Permitir testing del endpoint `POST /api/admin/evidence/upload` **sin autenticación** en ambiente Development, facilitando pruebas del frontend Next.js sin necesidad de configurar JWT/OAuth.

---

## ⚙️ Implementación

### AdminController.cs - Bypass Condicional

```csharp
[ApiController]
[Route("api/admin")]
#if !DEBUG
[Authorize(Roles = "MTO,Admin")] // Solo en Production
#endif
public class AdminController : ControllerBase
```

**Cómo funciona:**
- **Development (DEBUG)**: `[Authorize]` **NO se aplica** → endpoint público
- **Production (RELEASE)**: `[Authorize]` **SÍ se aplica** → requiere autenticación con roles MTO/Admin

---

## 🔒 Seguridad - Garantías

### 1. Activación Solo en DEBUG
La directiva `#if !DEBUG` garantiza que el bypass **solo existe** cuando:
- Visual Studio compila en modo **Debug**
- `dotnet run` sin `--configuration Release`

### 2. Production Protegido
Al publicar/desplegar:
```bash
dotnet publish -c Release
```
El compilador **elimina** la directiva `#if !DEBUG` y **incluye** `[Authorize]` en el ensamblado final.

### 3. Sin Riesgo de Fugas
- ✅ Compilación Release siempre tiene `[Authorize]`
- ✅ No hay flags de configuración que puedan fallar
- ✅ No depende de appsettings.json o variables de entorno
- ✅ El código de producción **no contiene** el bypass

---

## 🧪 Testing - Uso en Development

### Escenario 1: Frontend Next.js (http://localhost:3000)

**Formulario en `/evidence/upload`:**
```typescript
// lib/api-client.ts
async uploadEvidence(request: UploadEvidenceRequest) {
  const formData = new FormData();
  formData.append('memberId', request.memberId.toString());
  formData.append('vehicleId', request.vehicleId.toString());
  // ... otros campos

  const response = await fetch(
    `http://localhost:5000/api/admin/evidence/upload?eventId=${request.eventId}`,
    {
      method: 'POST',
      body: formData,
      // ✅ Sin Authorization header en Development
    }
  );

  return response.json();
}
```

**Resultado**: 
- ✅ Request exitoso (200 OK)
- ✅ Puntos calculados y retornados
- ✅ Evidencias guardadas en SQL Server

### Escenario 2: cURL (Testing Manual)

```bash
curl -X POST "http://localhost:5000/api/admin/evidence/upload?eventId=1" \
  -F "memberId=1" \
  -F "vehicleId=1" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@pilot.jpg" \
  -F "odometerCloseupPhoto=@odometer.jpg" \
  -F "odometerReading=12345.5" \
  -F "unit=Kilometers"
```

**Respuesta esperada:**
```json
{
  "message": "Asistencia confirmada con 120 puntos",
  "pointsAwarded": 120,
  "pointsPerEvent": 100,
  "pointsPerDistance": 20,
  "visitorClass": "LOCAL",
  "memberId": 1,
  "vehicleId": 1,
  "attendanceId": 42,
  "evidenceType": "START_YEAR"
}
```

### Escenario 3: Swagger UI

1. Abrir http://localhost:5000/swagger
2. Expandir **POST /api/admin/evidence/upload**
3. Click "Try it out"
4. ✅ No aparece el botón "Authorize" (porque no hay `[Authorize]`)
5. Llenar parámetros y ejecutar

---

## 🚀 Verificación de Configuración

### Verificar Modo de Compilación

**En Visual Studio:**
- Top menu: **Debug** → Configuration Manager
- Active solution configuration debe ser **Debug**

**En Terminal:**
```powershell
# Development - Sin autenticación
dotnet run --configuration Debug

# Production - Con autenticación
dotnet run --configuration Release
```

### Verificar Estado del Endpoint

**Test rápido:**
```bash
# Si retorna 200 → Bypass activo (Development)
# Si retorna 401 → Authorize activo (Production)
curl -X POST http://localhost:5000/api/admin/evidence/upload?eventId=1 \
  -F "memberId=1" \
  -F "vehicleId=1" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@test.jpg" \
  -F "odometerCloseupPhoto=@test.jpg" \
  -F "odometerReading=100" \
  -F "unit=Miles"
```

---

## 📋 Checklist - Antes de Production

Antes de desplegar a Azure/IIS/cualquier servidor:

- [ ] Compilar en modo **Release**:
  ```bash
  dotnet publish -c Release -o ./publish
  ```
- [ ] Verificar ensamblado tiene `[Authorize]`:
  ```bash
  # Decompile AdminController.dll con ILSpy/dotPeek
  # Debe mostrar: [Authorize(Roles = "MTO,Admin")]
  ```
- [ ] Configurar autenticación real (JWT/OAuth) en `Program.cs`
- [ ] Crear usuarios con roles MTO/Admin en el sistema de identidad
- [ ] Testing de production con tokens reales

---

## 🔄 Alternativa: Opción B (No Implementada)

Si en el futuro se requiere autenticación fake más realista en Development:

**Program.cs (Development only):**
```csharp
if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddAuthentication("FakeScheme")
        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("FakeScheme", null);
    
    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder("FakeScheme")
            .RequireAuthenticatedUser()
            .Build();
    });
}
else
{
    // Configuración real de JWT/OAuth
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => { /* ... */ });
}
```

**FakeAuthHandler.cs:**
```csharp
public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, "DevUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "MTO")
        };
        var identity = new ClaimsIdentity(claims, "FakeScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "FakeScheme");
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

**Ventajas de Opción B:**
- ✅ Mantiene pipeline de autenticación activo
- ✅ Permite testing de claims/roles
- ✅ Más cercano a comportamiento de producción

**Desventajas:**
- ❌ Más código y complejidad
- ❌ Requiere mantener dos pipelines (Dev vs Prod)
- ❌ Innecesario si solo se prueba funcionalidad, no seguridad

---

## 📚 Referencias

- [ASP.NET Core Conditional Compilation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives)
- [Authorization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction)
- [Test Authentication Handler](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

---

## ✅ Estado Actual

- ✅ AdminController configurado con `#if !DEBUG`
- ✅ Bypass activo en Development
- ✅ Production protegido con [Authorize]
- ✅ Frontend Next.js puede probar sin autenticación
- ⏳ Pendiente: Configurar JWT/OAuth real para Production
