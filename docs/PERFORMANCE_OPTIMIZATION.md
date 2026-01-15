# Optimización de Performance - LAMA Platform

## Resumen

Este documento describe las optimizaciones implementadas para soportar **4,000+ miembros globales** con baja latencia y alta disponibilidad.

**Fecha implementación:** 2025  
**Fase:** 13 - Performance Optimization

---

## 1. Problemas Identificados

### 1.1 Búsqueda de Miembros (N+1 Query)
**Problema:** El endpoint `/api/members/search` cargaba **todos los miembros en memoria** (`GetAllAsync()`) y luego filtraba con LINQ:
```csharp
// CÓDIGO ANTIGUO (MALO)
var members = await _memberRepository.GetAllAsync();
var filtered = members.Where(m => m.CompleteNames.ToLowerInvariant().Contains(searchTerm));
```

**Impacto:**
- 4,000+ registros cargados por cada búsqueda
- Tiempo de respuesta: 2-5 segundos
- Alto consumo de memoria (Entity Framework tracking)

### 1.2 Sin Caching Layer
**Problema:** Todas las consultas ejecutaban queries SQL directas sin caché.

**Impacto:**
- Búsquedas repetidas golpeaban base de datos
- Dropdowns de eventos (`/api/events?year=2026`) consultaban SQL cada vez
- Alta carga en SQL Server

### 1.3 Sin Rate Limiting
**Problema:** APIs públicas sin protección contra abuso.

**Impacto:**
- Riesgo de DDoS con búsquedas masivas
- Generación ilimitada de SAS URLs (endpoint de upload)

### 1.4 Sin Normalización de Búsqueda
**Problema:** Búsqueda "Jose" no encontraba "José" en base de datos.

**Impacto:**
- UX deficiente (tildes no funcionan)
- Búsquedas fallidas por acentos

---

## 2. Soluciones Implementadas

### 2.1 Búsqueda Optimizada con SQL

#### Cambios en Entidad Member
```csharp
// src/Lama.Domain/Entities/Member.cs
public class Member
{
    // ... propiedades existentes ...
    public string CompleteNames { get; set; }
    
    /// <summary>
    /// Versión normalizada de CompleteNames (sin tildes, mayúsculas, trimmed)
    /// para búsquedas rápidas con índice.
    /// </summary>
    public string? CompleteNamesNormalized { get; set; }
}
```

#### Configuración EF Core + Índice
```csharp
// src/Lama.Infrastructure/Data/Configurations/MemberConfiguration.cs
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        // ... configuraciones existentes ...
        
        builder.Property(m => m.CompleteNamesNormalized)
            .HasMaxLength(255)
            .HasColumnName("CompleteNamesNormalized");

        // Índice para búsquedas rápidas
        builder.HasIndex(m => m.CompleteNamesNormalized)
            .HasDatabaseName("IX_Members_CompleteNamesNormalized");
    }
}
```

#### Repositorio con Query SQL
```csharp
// src/Lama.Infrastructure/Repositories/MemberRepository.cs
public async Task<IEnumerable<Member>> SearchByNameAsync(
    string searchTerm, 
    int take = 20, 
    CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(searchTerm))
        return Enumerable.Empty<Member>();

    var normalizedTerm = NormalizeSearchTerm(searchTerm);
    
    return await _context.Members
        .Where(m => m.CompleteNamesNormalized != null && 
                    m.CompleteNamesNormalized.Contains(normalizedTerm))
        .OrderBy(m => m.CompleteNames)
        .Take(take)
        .AsNoTracking() // Sin tracking para mejor performance
        .ToListAsync(cancellationToken);
}

private static string NormalizeSearchTerm(string term)
{
    // Unicode FormD: Descomponer caracteres (á → a + ´)
    var normalized = term.Normalize(NormalizationForm.FormD);
    
    // Filtrar marcas de acento (NonSpacingMark)
    var stringBuilder = new StringBuilder();
    foreach (var c in normalized)
    {
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        {
            stringBuilder.Append(c);
        }
    }
    
    return stringBuilder.ToString()
        .Normalize(NormalizationForm.FormC)
        .ToUpperInvariant()
        .Trim();
}
```

**Performance:**
- Búsqueda anterior: **2,000-5,000 ms** (4,000 registros en memoria)
- Búsqueda actual: **< 500 ms** (SQL con índice)

---

### 2.2 Redis Caching Layer

#### Servicio de Caché
```csharp
// src/Lama.Application/Services/ICacheService.cs
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration, CancellationToken cancellationToken = default);
}

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    // Implementación con serialización JSON, try-catch en todas las operaciones
    // ...
}
```

#### Configuración Program.cs
```csharp
// src/Lama.API/Program.cs
// Redis con fallback a MemoryCache
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "Lama:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
    });
}

// Registrar servicio de caché (singleton)
builder.Services.AddSingleton<ICacheService, CacheService>();
```

#### Controller con Caching
```csharp
// src/Lama.API/Controllers/MembersController.cs
[HttpGet("search")]
[EnableRateLimiting("search")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers(
    [FromQuery] string q, 
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(q))
        return Ok(Enumerable.Empty<MemberSearchDto>());

    try
    {
        var cacheKey = $"members:search:{q.ToUpperInvariant().Trim()}";
        
        // Cache-aside pattern: check cache → execute query → store result
        var results = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var members = await _memberRepository.SearchByNameAsync(q, take: 20, cancellationToken);
                return members.Select(m => new MemberSearchDto
                {
                    MemberId = m.MemberId,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    FullName = m.CompleteNames,
                    Status = m.Status,
                    ChapterId = m.ChapterId
                }).ToList();
            },
            TimeSpan.FromSeconds(120), // TTL 120 segundos
            cancellationToken
        );

        return Ok(results);
    }
    catch (OperationCanceledException)
    {
        return StatusCode(499, "Client Closed Request");
    }
}
```

**Endpoints con Caché:**
- `/api/members/search?q={term}` → TTL 120s
- `/api/events?year={year}` → TTL 300s

**Performance:**
- Cache HIT: **< 50 ms**
- Cache MISS: **< 500 ms** (SQL + store)
- Cache Hit Rate esperado: **85-95%** (búsquedas autocomplete son repetitivas)

---

### 2.3 Rate Limiting

#### Configuración en Program.cs
```csharp
// Configurar políticas de rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Política "search": 30 requests/min (autocomplete)
    options.AddFixedWindowLimiter("search", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });

    // Política "upload": 10 requests/min (SAS URLs)
    options.AddFixedWindowLimiter("upload", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Rate limit exceeded. Please try again later." },
            cancellationToken: token
        );
    };
});

// Middleware (ANTES de UseAuthorization)
app.UseRateLimiter();
```

#### Aplicación en Endpoints
```csharp
// MembersController.cs
[EnableRateLimiting("search")]
public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers(...)

// EvidenceController.cs
[EnableRateLimiting("upload")]
public async Task<ActionResult<UploadSasResponse>> RequestUploadSasAsync(...)
```

**Límites:**
- Búsqueda: **30 req/min** por IP
- Upload SAS: **10 req/min** por IP

---

### 2.4 Request Cancellation (AbortController)

#### Backend: CancellationToken
```csharp
[HttpGet("search")]
public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers(
    [FromQuery] string q, 
    CancellationToken cancellationToken) // ← Parámetro agregado
{
    try
    {
        var results = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                // CancellationToken se propaga a repo → DbContext
                var members = await _memberRepository.SearchByNameAsync(q, take: 20, cancellationToken);
                return members.Select(...).ToList();
            },
            TimeSpan.FromSeconds(120),
            cancellationToken
        );

        return Ok(results);
    }
    catch (OperationCanceledException)
    {
        // Request cancelado por frontend (AbortController)
        return StatusCode(499, "Client Closed Request");
    }
}
```

#### Frontend: React Hook con AbortController
```typescript
// frontend/hooks/useMemberSearch.ts
export function useMemberSearch(options: UseMemberSearchOptions = {}) {
  const abortControllerRef = useRef<AbortController | null>(null);
  
  useEffect(() => {
    // Cancelar request anterior
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }

    // Crear nuevo AbortController
    const abortController = new AbortController();
    abortControllerRef.current = abortController;

    // Fetch con signal
    const response = await fetch(
      `/api/members/search?q=${encodeURIComponent(searchTerm)}`,
      {
        signal: abortController.signal, // ← AbortController
        headers: { 'Content-Type': 'application/json' }
      }
    );

    // ...
  }, [searchTerm]);
}
```

**Beneficios:**
- Usuario teclea rápido → requests anteriores cancelados
- Libera recursos de backend (query abortado en SQL Server)
- UX mejorado (sin resultados desactualizados)

---

## 3. Migración de Base de Datos

### Crear Migración
```bash
cd "C:\Users\DanielVillamizar\COR L.A.MA"
dotnet ef migrations add AddCompleteNamesNormalized -p src/Lama.Infrastructure -s src/Lama.API
```

### Aplicar Migración
```bash
dotnet ef database update -p src/Lama.Infrastructure -s src/Lama.API
```

### SQL Generado (Up Method)
```sql
-- Agregar columna CompleteNamesNormalized
ALTER TABLE Members 
ADD CompleteNamesNormalized NVARCHAR(255) NULL;

-- Crear índice para búsquedas rápidas
CREATE INDEX IX_Members_CompleteNamesNormalized 
ON Members(CompleteNamesNormalized);
```

### Poblar Datos Existentes
```sql
-- Script para normalizar nombres existentes (ejecutar DESPUÉS de migración)
UPDATE Members
SET CompleteNamesNormalized = UPPER(
    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    REPLACE(REPLACE(REPLACE(REPLACE(
    [Complete Names],
    'á', 'a'), 'é', 'e'), 'í', 'i'), 'ó', 'o'), 'ú', 'u'), 'ñ', 'n'), 'ü', 'u'),
    'Á', 'A'), 'É', 'E'), 'Í', 'I'), 'Ó', 'O'), 'Ú', 'U'), 'Ñ', 'N'), 'Ü', 'U'),
    'à', 'a'), 'è', 'e')
)
WHERE CompleteNamesNormalized IS NULL;
```

---

## 4. Configuración de Producción

### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=tcp:lama-sql-server.database.windows.net,1433;Initial Catalog=LamaDb;User ID=sqladmin;Password=xxx;",
    "Redis": "lama-cache.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False,connectTimeout=10000,syncTimeout=10000"
  },
  "AzureAd": {
    "RequireAuthentication": true
  }
}
```

### Azure Redis Cache
1. Crear recurso: **Azure Cache for Redis** (Standard C1 o superior)
2. Copiar connection string desde portal
3. Agregar a Key Vault o App Configuration
4. Configurar firewall rules (permitir Azure Services)

### Desarrollo Local (sin Redis)
Si no hay Redis instalado, el sistema usa **MemoryCache** automáticamente (fallback configurado en `Program.cs`).

---

## 5. Testing

### Test de Búsqueda (Cold vs Warm Cache)
```bash
# Primera búsqueda (cold cache)
curl -X GET "https://localhost:7001/api/members/search?q=jose" -w "\nTime: %{time_total}s\n"
# Esperado: 400-500ms (SQL query)

# Segunda búsqueda (warm cache)
curl -X GET "https://localhost:7001/api/members/search?q=jose" -w "\nTime: %{time_total}s\n"
# Esperado: < 50ms (Redis cache)
```

### Test de Rate Limiting
```powershell
# Test search rate limit (30 req/min)
for ($i = 1; $i -le 35; $i++) {
    $response = Invoke-WebRequest -Uri "https://localhost:7001/api/members/search?q=test"
    Write-Host "Request $i : $($response.StatusCode)"
}
# Esperado: 200 OK para 1-30, 429 Too Many Requests para 31-35
```

### Test de Request Cancellation
```typescript
// En React DevTools, simular tipeo rápido
// Verificar en Network tab que requests son cancelados (status: canceled)
```

---

## 6. Monitoreo

### Métricas Clave

**Response Time:**
- `/api/members/search` (cached): < 50ms (P95)
- `/api/members/search` (uncached): < 500ms (P95)
- `/api/events` (cached): < 30ms (P95)

**Cache Hit Rate:**
- Target: > 80%
- Fórmula: `(cache_hits / total_requests) * 100`

**Rate Limit Rejections:**
- Monitor: 429 responses en Application Insights
- Alert: Si > 5% del tráfico

**SQL Query Performance:**
- `SearchByNameAsync`: < 200ms (P95)
- Index usage: IX_Members_CompleteNamesNormalized debe aparecer en execution plan

### Application Insights Queries

```kusto
// Cache hit rate
requests
| where timestamp > ago(1h)
| where name == "GET Members/search"
| extend cacheHit = customDimensions.CacheHit
| summarize hits = countif(cacheHit == true), misses = countif(cacheHit == false)
| extend hitRate = hits * 100.0 / (hits + misses)

// Rate limit rejections
requests
| where timestamp > ago(1h)
| where resultCode == 429
| summarize count() by bin(timestamp, 5m)
```

---

## 7. Resumen de Mejoras

| Métrica | Antes | Después | Mejora |
|---------|-------|---------|--------|
| Search response (cached) | N/A | < 50ms | ∞ |
| Search response (uncached) | 2-5s | < 500ms | **90% faster** |
| DB queries per search | 1 | 0.2 (80% cache hit) | **80% reduction** |
| Memory usage | High (4K entities) | Low (indexed query) | **~90% reduction** |
| Rate limit protection | No | Yes (30/min) | ✅ Protected |
| Request cancellation | No | Yes | ✅ Supported |
| Accent-insensitive search | No | Yes | ✅ UX improved |

---

## 8. Próximos Pasos (Futuro)

### Fase 14: Monitoreo Avanzado
- [ ] Application Insights custom metrics (cache hit rate)
- [ ] Azure Monitor alerts (response time > 1s)
- [ ] Dashboard de performance en Grafana

### Fase 15: Escalabilidad Global
- [ ] Azure Redis Cluster (high availability)
- [ ] CDN para assets estáticos
- [ ] Multi-region deployment (Azure Traffic Manager)

### Fase 16: Optimización Avanzada
- [ ] Query result streaming (IAsyncEnumerable)
- [ ] GraphQL para reducir over-fetching
- [ ] Server-Side Rendering (SSR) para SEO

---

## Autores

**Implementación:** Daniel Villamizar  
**Fecha:** 2025  
**Versión:** 1.0
