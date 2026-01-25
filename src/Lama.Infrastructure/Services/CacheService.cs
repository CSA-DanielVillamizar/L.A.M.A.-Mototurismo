using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Lama.Application.Services;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación de ICacheService usando IDistributedCache (Redis con fallback a MemoryCache)
/// 
/// Ubicación: Infrastructure (implementación concreta)
/// Interfaz: Application.Services.ICacheService
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(
        IDistributedCache cache,
        ILogger<CacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogDebug("Cache miss para clave: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache hit para clave: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Cache unavailable, usando fallback para clave: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
            _logger.LogDebug("Valor cacheado con clave: {Key}, TTL: {TTL}s", key, expiration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Cache write failed (optional), continuando sin caché para clave: {Key}", key);
            // No lanzar excepción - el caché es opcional
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Valor removido del caché, clave: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover del caché, clave: {Key}", key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key, 
        Func<Task<T>> factory, 
        TimeSpan expiration, 
        CancellationToken cancellationToken = default)
    {
        // Intenta obtener del caché
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Si no existe, crea el valor
        _logger.LogDebug("Generando valor para caché: {Key}", key);
        var value = await factory();

        // Almacena en caché
        await SetAsync(key, value, expiration, cancellationToken);

        return value;
    }
}
