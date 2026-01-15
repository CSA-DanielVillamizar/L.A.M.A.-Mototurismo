# 🚀 FASE 13 - PERFORMANCE OPTIMIZATION - INSTRUCCIONES DE DEPLOYMENT

## ✅ Estado Actual

**COMPLETADO (Backend 100%):**
- ✅ SearchByNameAsync con query SQL optimizada
- ✅ CompleteNamesNormalized columna + índice (EF Core config)
- ✅ ICacheService implementado (Redis + MemoryCache fallback)
- ✅ Cache para /api/members/search (TTL 120s)
- ✅ Cache para /api/events (TTL 300s)
- ✅ Rate limiting configurado (search: 30/min, upload: 10/min)
- ✅ CancellationToken support (AbortController)
- ✅ Compilación exitosa (dotnet build)
- ✅ Migración EF Core generada
- ✅ Hook React con AbortController creado (frontend/hooks/useMemberSearch.ts)
- ✅ Script SQL de normalización (scripts/NormalizeMemberNames.sql)
- ✅ Documentación completa (docs/PERFORMANCE_OPTIMIZATION.md)

**PENDIENTE (Deployment):**
- ⏳ Aplicar migración a base de datos
- ⏳ Ejecutar script SQL de normalización
- ⏳ Configurar Redis connection string
- ⏳ Testing de performance

---

## 📋 PASO 1: Aplicar Migración

### 1.1 Revisar Migración Generada
```bash
# Navegar a directorio de migraciones
cd "C:\Users\DanielVillamizar\COR L.A.MA\src\Lama.Infrastructure\Migrations"

# Abrir archivo: *_AddCompleteNamesNormalized.cs
# Verificar que contiene:
#   - AddColumn: CompleteNamesNormalized (nvarchar(255), nullable)
#   - CreateIndex: IX_Members_CompleteNamesNormalized
```

### 1.2 Aplicar Migración (Development)
```bash
cd "C:\Users\DanielVillamizar\COR L.A.MA"

# Aplicar migración a LocalDB
dotnet ef database update -p src/Lama.Infrastructure -s src/Lama.API

# Verificar éxito (debe mostrar "Done")
```

### 1.3 Verificar en Base de Datos
```sql
-- Conectar a SQL Server Management Studio
-- Server: (localdb)\mssqllocaldb
-- Database: LamaDb

-- Verificar columna
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Members' AND COLUMN_NAME = 'CompleteNamesNormalized';

-- Esperado:
-- CompleteNamesNormalized | nvarchar | 255 | YES

-- Verificar índice
SELECT name, type_desc, is_unique
FROM sys.indexes
WHERE object_id = OBJECT_ID('Members') AND name = 'IX_Members_CompleteNamesNormalized';

-- Esperado:
-- IX_Members_CompleteNamesNormalized | NONCLUSTERED | 0
```

---

## 📋 PASO 2: Normalizar Datos Existentes

### 2.1 Ejecutar Script SQL
```bash
# Opción A: SQL Server Management Studio (RECOMENDADO)
# 1. Abrir: C:\Users\DanielVillamizar\COR L.A.MA\scripts\NormalizeMemberNames.sql
# 2. Conectar a: (localdb)\mssqllocaldb
# 3. Seleccionar database: LamaDb
# 4. Ejecutar (F5)
# 5. Verificar output: "Normalización completada exitosamente"

# Opción B: sqlcmd (línea de comandos)
sqlcmd -S "(localdb)\mssqllocaldb" -d LamaDb -i "C:\Users\DanielVillamizar\COR L.A.MA\scripts\NormalizeMemberNames.sql"
```

### 2.2 Verificar Normalización
```sql
-- Verificar ejemplos de normalización
SELECT TOP 10
    MemberId,
    [Complete Names] AS Original,
    CompleteNamesNormalized AS Normalizado
FROM Members;

-- Verificar que no queden NULL
SELECT COUNT(*) AS NullCount
FROM Members
WHERE CompleteNamesNormalized IS NULL;

-- Esperado: 0

-- Test de búsqueda (debe encontrar "José" buscando "jose")
SELECT *
FROM Members
WHERE CompleteNamesNormalized LIKE '%JOSE%';
```

---

## 📋 PASO 3: Configurar Redis (OPCIONAL para Dev)

### 3.1 Opción A: Redis Local (Docker - RECOMENDADO)
```bash
# Instalar Redis con Docker
docker run -d --name lama-redis -p 6379:6379 redis:7-alpine

# Verificar que está corriendo
docker ps

# Test de conexión
docker exec -it lama-redis redis-cli ping
# Esperado: PONG
```

### 3.2 Opción B: Redis en Windows (MSI)
```bash
# Descargar: https://github.com/microsoftarchive/redis/releases
# Instalar: Redis-x64-3.0.504.msi
# Iniciar servicio: "Redis" en services.msc
# Default port: 6379
```

### 3.3 Configurar Connection String
```bash
# Editar: src/Lama.API/appsettings.Development.json
# Agregar:
```

```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=(localdb)\\mssqllocaldb;Database=LamaDb;Trusted_Connection=true;",
    "Redis": "localhost:6379,abortConnect=false,connectTimeout=5000,syncTimeout=5000"
  }
}
```

### 3.4 Verificar Fallback (Sin Redis)
Si NO tienes Redis instalado, el sistema usará **MemoryCache** automáticamente.

**Log esperado al iniciar:**
```
info: Lama.Application.Services.CacheService[0]
      No Redis configured, using MemoryCache fallback
```

---

## 📋 PASO 4: Testing de Performance

### 4.1 Iniciar Aplicación
```bash
cd "C:\Users\DanielVillamizar\COR L.A.MA\src\Lama.API"
dotnet run
```

### 4.2 Test de Búsqueda (PowerShell)
```powershell
# Test 1: Cold cache (primera búsqueda)
Measure-Command {
    Invoke-RestMethod -Uri "https://localhost:7001/api/members/search?q=jose" -Method GET
}
# Esperado: < 500ms

# Test 2: Warm cache (segunda búsqueda - misma query)
Measure-Command {
    Invoke-RestMethod -Uri "https://localhost:7001/api/members/search?q=jose" -Method GET
}
# Esperado: < 50ms (Redis) o < 10ms (MemoryCache)

# Test 3: Búsqueda con tildes (debe funcionar igual)
Invoke-RestMethod -Uri "https://localhost:7001/api/members/search?q=josé" -Method GET
Invoke-RestMethod -Uri "https://localhost:7001/api/members/search?q=maria" -Method GET
```

### 4.3 Test de Rate Limiting
```powershell
# Test search rate limit (30 req/min)
1..35 | ForEach-Object {
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:7001/api/members/search?q=test" -Method GET
        Write-Host "Request $_ : $($response.StatusCode)"
    } catch {
        Write-Host "Request $_ : 429 Too Many Requests" -ForegroundColor Red
    }
}

# Esperado:
# Requests 1-30: 200 OK
# Requests 31-35: 429 Too Many Requests
```

### 4.4 Verificar Cache en Redis (si está instalado)
```bash
# Conectar a Redis CLI
docker exec -it lama-redis redis-cli

# Ver todas las keys
KEYS Lama:*

# Esperado:
# 1) "Lama:members:search:JOSE"
# 2) "Lama:events:all"

# Ver contenido de una key
GET "Lama:members:search:JOSE"

# Ver TTL (tiempo de vida)
TTL "Lama:members:search:JOSE"
# Esperado: ~120 segundos o menos
```

---

## 📋 PASO 5: Integrar Frontend (Opcional)

### 5.1 Usar Hook de React
```tsx
// En tu componente de búsqueda
import { useMemberSearch } from '../hooks/useMemberSearch';

function MemberSearchComponent() {
  const { 
    searchTerm, 
    setSearchTerm, 
    results, 
    isLoading, 
    error 
  } = useMemberSearch({
    debounceMs: 300,  // Esperar 300ms antes de buscar
    minChars: 2        // Mínimo 2 caracteres
  });

  return (
    <div>
      <input
        type="text"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        placeholder="Buscar miembro por nombre..."
      />
      
      {isLoading && <span>Buscando...</span>}
      {error && <span className="error">{error}</span>}
      
      <ul>
        {results.map(member => (
          <li key={member.memberId}>
            {member.fullName} - {member.status}
          </li>
        ))}
      </ul>
    </div>
  );
}
```

### 5.2 Verificar en Browser DevTools
1. Abrir Network tab
2. Teclear rápido en input de búsqueda
3. Verificar que requests anteriores se **cancelan** (status: "canceled")
4. Solo el último request debe completarse (status: 200)

---

## 📋 PASO 6: Monitoreo (Production)

### 6.1 Application Insights Queries
```kusto
// Cache hit rate
requests
| where timestamp > ago(1h)
| where name == "GET Members/search"
| extend cacheHit = customDimensions.CacheHit
| summarize hits = countif(cacheHit == "true"), misses = countif(cacheHit == "false")
| extend hitRate = hits * 100.0 / (hits + misses)

// Rate limit rejections
requests
| where timestamp > ago(1h)
| where resultCode == 429
| summarize count() by bin(timestamp, 5m), name
```

### 6.2 Performance Metrics
| Métrica | Target | Cómo medir |
|---------|--------|------------|
| Search response (cached) | < 50ms | Measure-Command en PowerShell |
| Search response (uncached) | < 500ms | Primera búsqueda después de limpiar cache |
| Cache hit rate | > 80% | Redis KEYS count vs total requests |
| Rate limit rejections | < 5% | Contador de 429 responses |

---

## 📋 PASO 7: Deployment a Production (Azure)

### 7.1 Crear Azure Redis Cache
```bash
# Azure Portal o CLI
az redis create \
  --name lama-cache \
  --resource-group lama-rg \
  --location eastus \
  --sku Standard \
  --vm-size C1

# Obtener connection string
az redis list-keys --name lama-cache --resource-group lama-rg
```

### 7.2 Configurar appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=tcp:lama-sql.database.windows.net,1433;Initial Catalog=LamaDb;User ID=xxx;Password=xxx;",
    "Redis": "lama-cache.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False"
  },
  "AzureAd": {
    "RequireAuthentication": true
  }
}
```

### 7.3 Aplicar Migración en Production
```bash
# Opción A: Azure SQL Management Studio
# Conectar a Azure SQL Server y ejecutar migración SQL

# Opción B: Desde dev con connection string de production
dotnet ef database update -p src/Lama.Infrastructure -s src/Lama.API --connection "Server=tcp:lama-sql.database.windows.net,1433;Initial Catalog=LamaDb;User ID=xxx;Password=xxx;"

# IMPORTANTE: Backup antes de ejecutar!
```

### 7.4 Ejecutar Script de Normalización en Production
```sql
-- Conectar a Azure SQL Server
-- Ejecutar: scripts/NormalizeMemberNames.sql
-- Verificar output
```

---

## ✅ Checklist Final

**Backend:**
- [ ] Migración aplicada (CompleteNamesNormalized + índice)
- [ ] Script SQL ejecutado (datos normalizados)
- [ ] Redis configurado (o fallback a MemoryCache)
- [ ] dotnet build exitoso (0 errores)
- [ ] dotnet run exitoso (app inicia)

**Testing:**
- [ ] Search response < 500ms (cold)
- [ ] Search response < 50ms (warm)
- [ ] Búsqueda con tildes funciona ("josé" → "JOSE")
- [ ] Rate limiting bloqueando request 31 (search)
- [ ] Rate limiting bloqueando request 11 (upload)
- [ ] Cache visible en Redis (keys Lama:*)

**Frontend (Opcional):**
- [ ] Hook useMemberSearch importado
- [ ] Debounce funcionando (300ms)
- [ ] Requests cancelados en Network tab
- [ ] Error 429 manejado con mensaje amigable

**Production:**
- [ ] Azure Redis Cache creado
- [ ] Connection string en Key Vault/App Configuration
- [ ] Migración aplicada en Azure SQL
- [ ] Datos normalizados en production
- [ ] Application Insights configurado

---

## 🆘 Troubleshooting

### Problema: Búsqueda no encuentra resultados
**Causa:** Datos no normalizados o índice no creado  
**Solución:**
```sql
-- Verificar datos
SELECT TOP 10 CompleteNamesNormalized FROM Members;
-- Si son NULL, ejecutar script NormalizeMemberNames.sql

-- Verificar índice
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('Members');
-- Si no existe IX_Members_CompleteNamesNormalized, re-aplicar migración
```

### Problema: Error "Redis connection failed"
**Causa:** Redis no instalado o connection string incorrecta  
**Solución:**
```bash
# Verificar Redis
docker ps | grep lama-redis

# Si no existe, instalar:
docker run -d --name lama-redis -p 6379:6379 redis:7-alpine

# Si existe pero no funciona, verificar connection string en appsettings.json
```

### Problema: Rate limiting no funciona
**Causa:** Middleware no registrado o atributo faltante  
**Solución:**
```csharp
// Verificar Program.cs tiene:
app.UseRateLimiter(); // ANTES de app.UseAuthorization()

// Verificar controller tiene:
[EnableRateLimiting("search")] // o "upload"
```

### Problema: Cache no guarda datos
**Causa:** IDistributedCache no inyectado o Redis desconectado  
**Solución:**
```bash
# Ver logs de aplicación
dotnet run

# Buscar línea:
# "Cache error: ..." → Redis no disponible, usando MemoryCache
# "Cache hit for key: ..." → Funcionando correctamente
```

---

## 📚 Documentación Adicional

- **Arquitectura completa:** [docs/PERFORMANCE_OPTIMIZATION.md](../docs/PERFORMANCE_OPTIMIZATION.md)
- **Frontend hook:** [frontend/hooks/useMemberSearch.ts](../frontend/hooks/useMemberSearch.ts)
- **Script SQL:** [scripts/NormalizeMemberNames.sql](../scripts/NormalizeMemberNames.sql)

---

**Autor:** Daniel Villamizar  
**Fecha:** 2025  
**Versión:** 1.0
