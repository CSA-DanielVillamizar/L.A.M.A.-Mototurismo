namespace Lama.Application.Services;

/// <summary>
/// Interfaz para servicio de caché distribuido (Redis con fallback a MemoryCache)
/// 
/// NOTA ARQUITECTÓNICA: Esta interfaz (abstracción) vive en Application.
/// La implementación concreta (CacheService) está en Infrastructure.Services
/// para mantener la separación arquitectónica (Clean Architecture).
/// 
/// Ubicación interfaz: Lama.Application.Services.ICacheService (abstracción)
/// Ubicación implementación: Lama.Infrastructure.Services.CacheService (concreta)
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
