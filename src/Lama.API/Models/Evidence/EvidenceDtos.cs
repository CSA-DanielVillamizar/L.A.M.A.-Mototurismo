namespace Lama.API.Models.Evidence;

/// <summary>
/// Request para solicitar SAS URLs para upload de evidencias
/// </summary>
public class EvidenceUploadRequestDto
{
    /// <summary>ID del evento (null para START_YEAR)</summary>
    public int? EventId { get; set; }

    /// <summary>ID del miembro que sube la evidencia</summary>
    public int MemberId { get; set; }

    /// <summary>ID del vehículo asociado</summary>
    public int VehicleId { get; set; }

    /// <summary>Tipo de evidencia: "START_YEAR" o "CUTOFF"</summary>
    public string EvidenceType { get; set; } = string.Empty;

    /// <summary>Content-Type de la foto del piloto (ej: "image/jpeg")</summary>
    public string PilotPhotoContentType { get; set; } = "image/jpeg";

    /// <summary>Content-Type de la foto del odómetro</summary>
    public string OdometerPhotoContentType { get; set; } = "image/jpeg";
}

/// <summary>
/// Response con SAS URLs para upload de evidencias
/// </summary>
public class EvidenceUploadResponseDto
{
    /// <summary>ID de correlación único para tracking</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>SAS URL para subir foto del piloto</summary>
    public string PilotPhotoSasUrl { get; set; } = string.Empty;

    /// <summary>SAS URL para subir foto del odómetro</summary>
    public string OdometerPhotoSasUrl { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del piloto (para usar en submit)</summary>
    public string PilotPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del odómetro (para usar en submit)</summary>
    public string OdometerPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Fecha de expiración de las SAS URLs (UTC)</summary>
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Request para enviar metadata de evidencia después de subir fotos a Blob
/// </summary>
public class EvidenceSubmitRequestDto
{
    /// <summary>ID de correlación recibido en upload-request</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>ID del evento (null para START_YEAR)</summary>
    public int? EventId { get; set; }

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>Tipo de evidencia: "START_YEAR" o "CUTOFF"</summary>
    public string EvidenceType { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del piloto (recibido en upload-request)</summary>
    public string PilotPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del odómetro (recibido en upload-request)</summary>
    public string OdometerPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Lectura del odómetro reportada por el miembro</summary>
    public decimal OdometerReading { get; set; }

    /// <summary>Unidad del odómetro: "Kilometers" o "Miles"</summary>
    public string OdometerUnit { get; set; } = "Kilometers";
}

/// <summary>
/// Response después de enviar metadata de evidencia
/// </summary>
public class EvidenceSubmitResponseDto
{
    /// <summary>ID de la evidencia creada</summary>
    public int EvidenceId { get; set; }

    /// <summary>ID de correlación</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>Estado de la evidencia</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>ID de la asistencia creada (si es CUTOFF)</summary>
    public int? AttendanceId { get; set; }

    /// <summary>Mensaje informativo</summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request para revisar (aprobar/rechazar) una evidencia
/// </summary>
public class EvidenceReviewRequestDto
{
    /// <summary>ID de la evidencia a revisar</summary>
    public int EvidenceId { get; set; }

    /// <summary>Acción: "approve" o "reject"</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Notas del revisor (obligatorio si es reject)</summary>
    public string? ReviewNotes { get; set; }
}

/// <summary>
/// Response después de revisar una evidencia
/// </summary>
public class EvidenceReviewResponseDto
{
    /// <summary>ID de la evidencia revisada</summary>
    public int EvidenceId { get; set; }

    /// <summary>Nuevo estado</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Mensaje informativo</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>ID de la asistencia actualizada (si fue aprobada y es CUTOFF)</summary>
    public int? AttendanceId { get; set; }

    /// <summary>Puntos otorgados (si fue aprobada y es CUTOFF)</summary>
    public int? PointsAwarded { get; set; }
}

/// <summary>
/// DTO para listar evidencias con información básica
/// </summary>
public class EvidenceListItemDto
{
    public int Id { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehiclePlate { get; set; } = string.Empty;
    public int? EventId { get; set; }
    public string? EventName { get; set; }
    public string EvidenceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal OdometerReading { get; set; }
    public string OdometerUnit { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
    
    /// <summary>URLs de lectura con SAS para las fotos (generadas bajo demanda)</summary>
    public string? PilotPhotoUrl { get; set; }
    public string? OdometerPhotoUrl { get; set; }
}
