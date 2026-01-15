namespace Lama.Application.Services;

/// <summary>
/// Interfaz para acceso a configuración global desde base de datos
/// </summary>
public interface IAppConfigProvider
{
    /// <summary>Obtiene un valor de configuración como entero</summary>
    Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un valor de configuración como doble</summary>
    Task<double> GetDoubleAsync(string key, double defaultValue = 0.0, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un valor de configuración como string</summary>
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un valor de configuración como booleano</summary>
    Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);
}
