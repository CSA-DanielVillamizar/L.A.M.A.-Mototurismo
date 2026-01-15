using Microsoft.EntityFrameworkCore;
using Lama.Infrastructure.Data;
using Lama.Application.Services;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación de IAppConfigProvider que lee desde base de datos
/// </summary>
public class AppConfigProvider(LamaDbContext context) : IAppConfigProvider
{
    private readonly LamaDbContext _context = context;

    public async Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        if (config == null || string.IsNullOrEmpty(config.Value))
            return defaultValue;

        return int.TryParse(config.Value, out var result) ? result : defaultValue;
    }

    public async Task<double> GetDoubleAsync(string key, double defaultValue = 0.0, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        if (config == null || string.IsNullOrEmpty(config.Value))
            return defaultValue;

        return double.TryParse(config.Value, out var result) ? result : defaultValue;
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        return config?.Value;
    }

    public async Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        if (config == null || string.IsNullOrEmpty(config.Value))
            return defaultValue;

        return bool.TryParse(config.Value, out var result) ? result : defaultValue;
    }
}
