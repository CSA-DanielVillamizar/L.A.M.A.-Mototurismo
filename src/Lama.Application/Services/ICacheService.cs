using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Lama.Application.Services;

/// <summary>
/// Servicio de caché distribuido (Redis) para optimización de performance
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtiene un valor cacheado por clave
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Almacena un valor en caché con TTL
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un valor del caché
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene o crea un valor en caché (patrón cache-aside)
    /// </summary>
    Task<T> GetOrCreateAsync<T>(
        string key, 
        Func<Task<T>> factory, 
        TimeSpan expiration, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementación de ICacheService usando IDistributedCache (Redis)
/// </summary>
public class CacheService : ICacheService
{
    private readonly Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;
    private readonly Microsoft.Extensions.Logging.ILogger<CacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(
        Microsoft.Extensions.Caching.Distributed.IDistributedCache cache,
        Microsoft.Extensions.Logging.ILogger<CacheService> logger)
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
            _logger.LogError(ex, "Error al leer del caché, clave: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
            
            var options = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
            _logger.LogDebug("Valor cacheado con clave: {Key}, TTL: {TTL}s", key, expiration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al escribir en caché, clave: {Key}", key);
            // No lanzar excepción - el caché es opcional
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Clave eliminada del caché: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar del caché, clave: {Key}", key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key, 
        Func<Task<T>> factory, 
        TimeSpan expiration, 
        CancellationToken cancellationToken = default)
    {
        // Intentar obtener del caché
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        // No existe en caché, ejecutar factory
        _logger.LogDebug("Cache miss, ejecutando factory para clave: {Key}", key);
        var value = await factory();

        // Cachear resultado (si no es null)
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }
}
