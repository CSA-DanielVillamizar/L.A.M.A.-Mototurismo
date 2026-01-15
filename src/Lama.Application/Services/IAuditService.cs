using Lama.Domain.Enums;
using Lama.Domain.Entities;

namespace Lama.Application.Services;

/// <summary>
/// Servicio de auditoría para registrar acciones auditables del sistema.
/// Proporciona métodos para registrar cambios en entidades, acciones administrativas, y eventos de seguridad.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Registra una acción auditable en el sistema.
    /// </summary>
    /// <param name="tenantId">ID del inquilino.</param>
    /// <param name="actorExternalSubjectId">Subject externo del actor (ej: claim sub de Azure B2C).</param>
    /// <param name="actorMemberId">ID del miembro (opcional si la acción fue hecha por usuario no-miembro).</param>
    /// <param name="action">Tipo de acción realizada.</param>
    /// <param name="entityType">Tipo de entidad afectada.</param>
    /// <param name="entityId">ID específico de la entidad (ej: "123" para Evidence con Id=123).</param>
    /// <param name="beforeJson">JSON del estado anterior (null para creaciones).</param>
    /// <param name="afterJson">JSON del estado nuevo (null para eliminaciones lógicas).</param>
    /// <param name="notes">Notas adicionales opcionales sobre la acción.</param>
    /// <param name="correlationId">ID de correlación para rastreo distribuido.</param>
    /// <param name="ipAddress">Dirección IP del cliente.</param>
    /// <param name="userAgent">User-Agent del cliente.</param>
    /// <returns>Task completado una vez que el registro se haya guardado en BD.</returns>
    Task LogAsync(
        Guid tenantId,
        string actorExternalSubjectId,
        int? actorMemberId,
        AuditActionType action,
        AuditEntityType entityType,
        string entityId,
        string? beforeJson = null,
        string? afterJson = null,
        string? notes = null,
        string? correlationId = null,
        string? ipAddress = null,
        string? userAgent = null);

    /// <summary>
    /// Obtiene registros de auditoría filtrados por miembro.
    /// </summary>
    /// <param name="tenantId">ID del inquilino.</param>
    /// <param name="actorMemberId">ID del miembro cuyas acciones se quieren auditar.</param>
    /// <param name="take">Número máximo de registros (default 100, max 1000).</param>
    /// <returns>Lista de registros de auditoría del miembro.</returns>
    Task<IEnumerable<AuditLogDto>> GetAuditsByMemberAsync(Guid tenantId, int actorMemberId, int take = 100);

    /// <summary>
    /// Obtiene registros de auditoría filtrados por evento/entidad.
    /// </summary>
    /// <param name="tenantId">ID del inquilino.</param>
    /// <param name="entityType">Tipo de entidad.</param>
    /// <param name="entityId">ID específico de la entidad.</param>
    /// <param name="take">Número máximo de registros (default 100, max 1000).</param>
    /// <returns>Lista de registros de auditoría para la entidad especificada.</returns>
    Task<IEnumerable<AuditLogDto>> GetAuditsByEntityAsync(
        Guid tenantId,
        AuditEntityType entityType,
        string entityId,
        int take = 100);

    /// <summary>
    /// Obtiene registros de auditoría filtrados por ID de correlación (para rastreo distribuido).
    /// </summary>
    /// <param name="correlationId">ID de correlación a rastrear.</param>
    /// <returns>Lista de registros de auditoría con el ID de correlación especificado.</returns>
    Task<IEnumerable<AuditLogDto>> GetAuditsByCorrelationIdAsync(string correlationId);

    /// <summary>
    /// Obtiene un resumen de auditoría de los últimos N días.
    /// </summary>
    /// <param name="tenantId">ID del inquilino.</param>
    /// <param name="days">Número de días hacia atrás (default 30).</param>
    /// <returns>Resumen de auditoría incluyendo conteo por tipo de acción y entidad.</returns>
    Task<AuditSummaryDto> GetAuditSummaryAsync(Guid tenantId, int days = 30);
}

/// <summary>
/// DTO para transferir datos de auditoría hacia el cliente.
/// </summary>
public class AuditLogDto
{
    /// <summary>Identificador único del registro de auditoría.</summary>
    public int Id { get; set; }

    /// <summary>Subject externo del actor.</summary>
    public string ActorExternalSubjectId { get; set; } = string.Empty;

    /// <summary>ID del miembro (si está disponible).</summary>
    public int? ActorMemberId { get; set; }

    /// <summary>Tipo de acción realizada.</summary>
    public AuditActionType Action { get; set; }

    /// <summary>Tipo de entidad afectada.</summary>
    public AuditEntityType EntityType { get; set; }

    /// <summary>ID de la entidad específica.</summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>Estado anterior en JSON.</summary>
    public string? BeforeJson { get; set; }

    /// <summary>Estado nuevo en JSON.</summary>
    public string? AfterJson { get; set; }

    /// <summary>Notas adicionales.</summary>
    public string? Notes { get; set; }

    /// <summary>ID de correlación.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Dirección IP del cliente.</summary>
    public string? IpAddress { get; set; }

    /// <summary>User-Agent del cliente.</summary>
    public string? UserAgent { get; set; }

    /// <summary>Marca de tiempo cuando se registró la acción.</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Resumen de auditoría para análisis agregado.
/// </summary>
public class AuditSummaryDto
{
    /// <summary>ID del inquilino.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Total de registros en el período.</summary>
    public int TotalRecords { get; set; }

    /// <summary>Período cubierto (días).</summary>
    public int DaysCovered { get; set; }

    /// <summary>Conteo de acciones por tipo de acción.</summary>
    public Dictionary<string, int> ActionCounts { get; set; } = new();

    /// <summary>Conteo de acciones por tipo de entidad.</summary>
    public Dictionary<string, int> EntityTypeCounts { get; set; } = new();

    /// <summary>Top 10 miembros más activos (por número de acciones).</summary>
    public List<MemberActivityDto> TopActiveMembers { get; set; } = new();

    /// <summary>Acciones de rechazo/error (potencialmente sospechosas).</summary>
    public List<AuditLogDto> SuspiciousActions { get; set; } = new();
}

/// <summary>
/// Actividad de miembro para el resumen de auditoría.
/// </summary>
public class MemberActivityDto
{
    /// <summary>ID del miembro.</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre del miembro.</summary>
    public string MemberName { get; set; } = string.Empty;

    /// <summary>Número total de acciones.</summary>
    public int ActionCount { get; set; }

    /// <summary>Última acción registrada.</summary>
    public DateTime LastActionAt { get; set; }
}
