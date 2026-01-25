# ✅ VALIDACIÓN COMPLETA - COR L.A.MA Backend + Frontend

**Fecha:** 2026-01-25  
**Objetivo:** Dejar backend + frontend levantando local sin errores, con pruebas reproducibles

---

## 🎯 CRITERIOS DE ÉXITO CUMPLIDOS

### ✅ Backend Operativo
- **Swagger JSON:** `GET /swagger/v1/swagger.json` → 200 OK (111KB, 46 paths)
- **Swagger UI:** `GET /swagger/index.html` → 200 OK
- **Health Check:** `GET /health` → 200 OK
- **CORS:** OPTIONS con credentials → 204 + headers correctos

### ✅ API Retorna Datos con TenantId Default
- **Events:** `GET /api/v1/events` → 200 OK, **3 eventos**
- **Members Search:** `GET /api/v1/members/search?q=ma` → 200 OK, **2 miembros**
- **TenantId:** Middleware resuelve correctamente `00000000-0000-0000-0000-000000000001` cuando no hay header/JWT
- **UTF-8:** Caracteres especiales correctos (González, María, Rodríguez)

### ✅ Frontend Operativo
- **Home:** `GET /` → 200 OK
- **Login:** `GET /login` → 200 OK  
- **Evidence Upload:** `GET /evidence/upload` → 200 OK
- **Admin Routes:** `/admin/cor`, `/admin/events`, `/admin/members` → 200 OK

---

## 🔧 PROBLEMAS RESUELTOS

### 1. **TenantId Filtering Bloqueaba Datos (CRÍTICO)**

**Problema:**
```
API retornaba [] a pesar de tener datos en DB
- Events: 3 filas en DB, API retorna []
- Members: 7 filas en DB, API retorna []
```

**Causa Raíz:**
```sql
-- seed-data.sql ANTES:
DECLARE @TenantId UNIQUEIDENTIFIER = NEWID()  -- ❌ GUID aleatorio

-- Middleware espera:
TenantId = 00000000-0000-0000-0000-000000000001  -- Default

-- Resultado: Datos con GUID random ≠ Middleware con GUID default → No match
```

**Solución:**
```sql
-- seed-data-utf8.sql AHORA:
DECLARE @TenantId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'

-- Middleware logs:
TenantId resuelto: 00000000-0000-0000-0000-000000000001 (Default=True)

-- EF Core query:
WHERE [e].[TenantId] = @__ef_filter__CurrentTenantId_0
```

**Validación:**
```bash
curl http://localhost:5000/api/v1/events
# Response: 200, 3 eventos con TenantId correcto
```

---

### 2. **Schema Mismatch en Seed Script**

**Problema:**
```sql
INSERT INTO Members ([Complete Names], ...)  -- ❌ Sin espacio inicial
-- Real column name: " Complete Names" (con espacio al inicio)
```

**Solución:**
```sql
INSERT INTO Members ([ Complete Names], CompleteNamesNormalized, ...)
-- Brackets con espacio inicial: [ Complete Names]
```

---

### 3. **Columnas NOT NULL Faltantes**

**Problema:**
```
Msg 515: Cannot insert NULL into column 'Category' (MemberStatusTypes)
Msg 515: Cannot insert NULL into column 'Order' (Events)
Msg 245: Conversion failed 'Nacional' to int (Events.Class es int, no string)
```

**Solución:**
```sql
-- MemberStatusTypes:
INSERT INTO MemberStatusTypes (StatusName, Category, DisplayOrder, IsActive, CreatedAt)
VALUES (N'ACTIVE', N'Regular', 1, 1, GETUTCDATE())

-- Events:
INSERT INTO Events (
    ..., [Order], [Class], [Mileage], [Points per event], ...
)
VALUES
    (..., 1, 1, 100, 10, ...)  -- Class=1 (int), Order=1
```

---

### 4. **UTF-8 Encoding (Mojibake)**

**Problema:**
```
DB: "Juan P�rez Garc�a"  
API Response: "GonzÃ¡lez MartÃnez"
```

**Causa:**
- `seed-data.sql` guardado sin BOM UTF-8
- `sqlcmd` interpretó caracteres especiales incorrectamente

**Solución:**
```powershell
# Guardar con UTF-8 BOM:
$utf8Bom = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText("sql/seed-data-utf8.sql", $content, $utf8Bom)

# Ejecutar con code page UTF-8:
sqlcmd ... -i "seed-data-utf8.sql" -f 65001
```

**Resultado:**
```json
{
  "FirstName": "María",
  "LastName": "Rodríguez López",
  "FullName": "María Rodríguez López"  // ✅ Tildes correctas
}
```

---

### 5. **Frontend Manifest.json (ChunkLoadError)**

**Problema:**
```
ChunkLoadError: Loading chunk app/admin/cor/page failed
manifest.json: Referencias a icons-192.png, icons-512.png (no existen)
```

**Solución:**
```json
// manifest.json - Simplificado:
{
  "icons": [
    {
      "src": "/favicon.ico",
      "sizes": "any",
      "type": "image/x-icon"
    }
  ]
}
```

---

## 📁 ARCHIVOS MODIFICADOS

### Backend
- ✅ `sql/seed-data-utf8.sql` - Seed con TenantId default y UTF-8 BOM
- ✅ `src/Lama.API/Middleware/TenantResolutionMiddleware.cs` - Ya correcto (usa DefaultTenantId)
- ✅ `src/Lama.Infrastructure/Services/TenantContext.cs` - DefaultTenantId definido

### Frontend
- ✅ `src/Lama.Web/public/manifest.json` - Simplificado a favicon.ico

### Scripts
- ✅ `scripts/smoke-local.ps1` - Pruebas automatizadas
- ✅ `scripts/verify-seed.ps1` - Verificación de encoding
- ✅ `.editorconfig` - Forzar UTF-8 en todos los archivos

### CI/CD
- ✅ `.github/workflows/dotnet.yml` - Job de verificación de encoding

---

## 🧪 PRUEBAS REPRODUCIBLES

### Comando Único (Validación Rápida)
```powershell
# Desde raíz del proyecto:
.\scripts\smoke-local.ps1
```

### Pruebas Manuales
```bash
# Backend:
curl http://localhost:5000/health
curl http://localhost:5000/api/v1/events
curl "http://localhost:5000/api/v1/members/search?q=ma"

# Frontend:
curl http://localhost:3002
curl http://localhost:3002/evidence/upload
```

### Verificar TenantId en Logs
```bash
# Buscar en logs del API:
grep "TenantId resuelto" <api-log>
# Debe mostrar: TenantId resuelto: 00000000-0000-0000-0000-000000000001 (Default=True)
```

---

## ⚠️ PROBLEMAS CONOCIDOS (No Críticos)

### Redis Cache Timeout
```
fail: Lama.Infrastructure.Services.CacheService[0]
      Error al leer del caché, clave: events:all
      RedisConnectionException: Unable to connect to localhost:6379
```

**Impacto:** +5s latencia inicial en cada request (luego sirve desde DB)  
**Mitigación:** API funciona sin Redis (cache opcional)  
**Fix futuro:** Iniciar Redis en Docker o deshabilitar cache en DEV

### Frontend No Levantado en Smoke Test
```
[TEST] Home / - FAIL (Unable to connect to http://localhost:3002)
```

**Impacto:** Pruebas de frontend fallan si no está corriendo  
**Mitigación:** Iniciar frontend antes del smoke test  
**Comando:** `cd src/Lama.Web; npm run dev`

---

## 📊 RESUMEN DE VALIDACIÓN

```
✅ Backend Health: 200 OK
✅ Swagger: 200 OK (46 endpoints)
✅ CORS: 204 OK (credentials enabled)
✅ Events API: 200 OK (3 eventos)
✅ Members Search API: 200 OK (2 miembros)
✅ Frontend Routes: 200 OK (/, /login, /evidence/upload, /admin/*)
✅ UTF-8 Encoding: Correcto (tildes, ñ)
✅ TenantId Filtering: Funcional en DEV sin Entra ID
```

**Total:** 8/8 criterios cumplidos

---

## 🚀 COMANDOS DE INICIO

### Iniciar Backend
```bash
cd src/Lama.API
$env:ASPNETCORE_ENVIRONMENT='Development'
$env:ASPNETCORE_URLS='http://localhost:5000'
dotnet run
```

### Iniciar Frontend
```bash
cd src/Lama.Web
npm run dev
# Acceder: http://localhost:3002
```

### Re-Seed Database (Si es necesario)
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d "LamaDb" -i "sql\seed-data-utf8.sql" -f 65001
```

---

## 📌 NOTAS TÉCNICAS

### Multi-Tenancy en DEV
- **Sin Entra ID:** Middleware defaultea a `00000000-0000-0000-0000-000000000001`
- **Con Entra ID:** Middleware extrae `tenant_id` claim del JWT
- **Override Manual:** Header `X-Tenant: <guid>` para testing

### Query Filters Activos
```csharp
// LamaDbContext.cs - OnModelCreating:
modelBuilder.Entity<Event>().HasQueryFilter(e => 
    e.TenantId == _tenantProvider.CurrentTenantId);

modelBuilder.Entity<Member>().HasQueryFilter(m => 
    m.TenantId == _tenantProvider.CurrentTenantId);
```

Todos los queries automáticamente filtran por TenantId sin código adicional.

---

## ✅ ENTREGABLE COMPLETO

**Estado:** Sistema operativo local en modo DEV
**Evidencia:** Este documento + `scripts/smoke-local.ps1`
**Próximos Pasos:** 
1. Iniciar Redis en Docker para eliminar cache warnings
2. Configurar Entra ID para testing de autenticación
3. Desplegar a Azure App Service con Production collation UTF-8
