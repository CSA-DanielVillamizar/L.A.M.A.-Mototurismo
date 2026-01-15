namespace Lama.Application.Abstractions;

/// <summary>
/// Interfaz para resolver el Tenant actual en el contexto de la solicitud HTTP
/// Respeta Clean Architecture: Application no depende de infraestructura específica
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// ID del tenant actual (GUID)
    /// En desarrollo/test, retorna LAMA_DEFAULT si no se especifica
    /// </summary>
    Guid CurrentTenantId { get; }

    /// <summary>
    /// Nombre del tenant (opcional, solo para logging/auditoría)
    /// </summary>
    string? CurrentTenantName { get; }

    /// <summary>
    /// Indica si el tenant actual es el tenant por defecto
    /// </summary>
    bool IsDefaultTenant { get; }
}
