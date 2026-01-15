namespace Lama.Domain.Entities;

/// <summary>
/// Tipos de evidencia que pueden subirse
/// </summary>
public enum EvidenceType
{
    /// <summary>Evidencia de inicio de año (foto piloto + odómetro)</summary>
    START_YEAR = 0,

    /// <summary>Evidencia de asistencia a evento (foto piloto + odómetro)</summary>
    CUTOFF = 1
}

/// <summary>
/// Estados de revisión de evidencia
/// </summary>
public enum EvidenceStatus
{
    /// <summary>Pendiente de revisión por admin/MTO</summary>
    PENDING_REVIEW = 0,

    /// <summary>Aprobada por admin/MTO</summary>
    APPROVED = 1,

    /// <summary>Rechazada por admin/MTO</summary>
    REJECTED = 2
}

/// <summary>
/// Representa una evidencia fotográfica subida por un miembro
/// (foto de piloto + foto de odómetro)
/// </summary>
public class Evidence
{
    /// <summary>ID único de la evidencia</summary>
    public int Id { get; set; }

    /// <summary>Tenant al que pertenece esta evidencia (multi-tenancy)</summary>
    public Guid TenantId { get; set; }

    /// <summary>ID de correlación único para tracking (generado por backend)</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>ID del miembro que sube la evidencia</summary>
    public int MemberId { get; set; }

    /// <summary>Navegación al miembro</summary>
    public Member? Member { get; set; }

    /// <summary>ID del vehículo asociado</summary>
    public int VehicleId { get; set; }

    /// <summary>Navegación al vehículo</summary>
    public Vehicle? Vehicle { get; set; }

    /// <summary>ID del evento asociado (si es CUTOFF), null si es START_YEAR</summary>
    public int? EventId { get; set; }

    /// <summary>Navegación al evento</summary>
    public Event? Event { get; set; }

    /// <summary>Tipo de evidencia (START_YEAR o CUTOFF)</summary>
    public EvidenceType EvidenceType { get; set; }

    /// <summary>Estado de revisión de la evidencia</summary>
    public EvidenceStatus Status { get; set; }

    /// <summary>Ruta del blob de la foto del piloto en Azure Blob Storage</summary>
    public string PilotPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Ruta del blob de la foto del odómetro en Azure Blob Storage</summary>
    public string OdometerPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Lectura del odómetro reportada por el miembro</summary>
    public decimal OdometerReading { get; set; }

    /// <summary>Unidad del odómetro (Kilometers o Miles)</summary>
    public string OdometerUnit { get; set; } = "Kilometers";

    /// <summary>Fecha y hora de creación de la evidencia (UTC)</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Fecha y hora de revisión (UTC), null si aún no revisado</summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>ID del admin/MTO que revisó, null si aún no revisado</summary>
    public string? ReviewedBy { get; set; }

    /// <summary>Notas del revisor (motivo de rechazo o comentarios)</summary>
    public string? ReviewNotes { get; set; }

    /// <summary>ID de la asistencia generada (si es CUTOFF y fue aprobada)</summary>
    public int? AttendanceId { get; set; }

    /// <summary>Navegación a la asistencia generada</summary>
    public Attendance? Attendance { get; set; }

    /// <summary>Fecha y hora de última actualización (UTC)</summary>
    public DateTime UpdatedAt { get; set; }
}
