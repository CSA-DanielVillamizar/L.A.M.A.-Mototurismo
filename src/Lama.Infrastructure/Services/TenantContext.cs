using Lama.Application.Abstractions;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Contexto de Tenant - Holder scoped para el tenant actual en la solicitud
/// Inyectado en middleware y disponible en toda la aplicación
/// </summary>
public class TenantContext : ITenantProvider
{
    /// <summary>
    /// ID del tenant por defecto (LAMA_DEFAULT)
    /// </summary>
    public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Nombre del tenant por defecto
    /// </summary>
    public const string DefaultTenantName = "LAMA_DEFAULT";

    private Guid _currentTenantId = DefaultTenantId;
    private string? _currentTenantName = DefaultTenantName;

    /// <summary>
    /// ID del tenant actual
    /// </summary>
    public Guid CurrentTenantId
    {
        get => _currentTenantId;
        set => _currentTenantId = value;
    }

    /// <summary>
    /// Nombre del tenant actual (opcional)
    /// </summary>
    public string? CurrentTenantName
    {
        get => _currentTenantName;
        set => _currentTenantName = value;
    }

    /// <summary>
    /// Indica si el tenant actual es el tenant por defecto
    /// </summary>
    public bool IsDefaultTenant => _currentTenantId == DefaultTenantId;

    /// <summary>
    /// Resetea el contexto al tenant por defecto
    /// Útil para testing
    /// </summary>
    public void ResetToDefault()
    {
        _currentTenantId = DefaultTenantId;
        _currentTenantName = DefaultTenantName;
    }
}
