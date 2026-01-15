using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Entidad que registra todas las acciones auditables del sistema.
/// Proporciona trazabilidad completa para cumplimiento normativo y análisis de auditoría.
/// </summary>
public class AuditLog
{
    /// <summary>Identificador único del registro de auditoría.</summary>
    public int Id { get; set; }

    /// <summary>ID del inquilino (tenant) al que pertenece este registro.</summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Subject externo del actor (claim sub de Azure B2C).
    /// Ej: "00000000-0000-0000-0000-000000000000" o email del usuario B2C.
    /// </summary>
    public string ActorExternalSubjectId { get; set; } = string.Empty;

    /// <summary>
    /// ID del miembro (si está disponible) que realizó la acción.
    /// Puede ser null si la acción fue realizada por un usuario no miembro.
    /// </summary>
    public int? ActorMemberId { get; set; }

    /// <summary>Tipo de acción auditada (aprobación, rechazo, actualización, etc.).</summary>
    public AuditActionType Action { get; set; }

    /// <summary>Tipo de entidad que fue modificada (Evidence, Attendance, Vehicle, Member, etc.).</summary>
    public AuditEntityType EntityType { get; set; }

    /// <summary>ID de la entidad específica que fue afectada por la acción.</summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Representación JSON del estado anterior de la entidad (antes de la modificación).
    /// Null si es una creación nueva.
    /// </summary>
    public string? BeforeJson { get; set; }

    /// <summary>
    /// Representación JSON del estado nuevo de la entidad (después de la modificación).
    /// Null si es una eliminación lógica.
    /// </summary>
    public string? AfterJson { get; set; }

    /// <summary>
    /// Notas opcionales adicionales sobre la acción.
    /// Ej: "Rechazada por falta de documentación", "Odómetro corregido manualmente".
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>ID de correlación único para rastrear una solicitud a través de todo el sistema distribuido.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Dirección IP del cliente que realizó la acción.</summary>
    public string? IpAddress { get; set; }

    /// <summary>User-Agent del navegador o cliente que realizó la acción.</summary>
    public string? UserAgent { get; set; }

    /// <summary>Marca de tiempo (UTC) cuando se registró la acción.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
