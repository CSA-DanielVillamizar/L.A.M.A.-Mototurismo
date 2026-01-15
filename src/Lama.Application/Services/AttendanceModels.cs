using Lama.Domain.Entities;
using Lama.Domain.Enums;

namespace Lama.Application.Services;

/// <summary>
/// DTO para solicitud de carga de evidencia fotográfica
/// </summary>
public class UploadEvidenceRequest
{
    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>Tipo de evidencia (START_YEAR, CUTOFF)</summary>
    public string EvidenceType { get; set; } = string.Empty;

    /// <summary>Foto: Piloto con moto</summary>
    public required Stream PilotWithBikePhotoStream { get; set; }

    /// <summary>Nombre del archivo: Piloto con moto</summary>
    public string PilotWithBikePhotoFileName { get; set; } = "pilot_bike.jpg";

    /// <summary>Foto: Odómetro close-up</summary>
    public required Stream OdometerCloseupPhotoStream { get; set; }

    /// <summary>Nombre del archivo: Odómetro</summary>
    public string OdometerCloseupPhotoFileName { get; set; } = "odometer.jpg";

    /// <summary>Lectura del odómetro</summary>
    public double OdometerReading { get; set; }

    /// <summary>Unidad (Miles, Kilometers)</summary>
    public string Unit { get; set; } = "Miles";

    /// <summary>Fecha de lectura (opcional, por defecto hoy)</summary>
    public DateOnly? ReadingDate { get; set; }

    /// <summary>Notas adicionales</summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Resultado de confirmación de asistencia
/// </summary>
public class AttendanceConfirmationResult
{
    /// <summary>Indica si la operación fue exitosa</summary>
    public bool Success { get; set; }

    /// <summary>Mensaje de resultado</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>ID de la asistencia procesada</summary>
    public int? AttendanceId { get; set; }

    /// <summary>Puntos otorgados por evento</summary>
    public int? PointsPerEvent { get; set; }

    /// <summary>Puntos otorgados por distancia</summary>
    public int? PointsPerDistance { get; set; }

    /// <summary>Puntos totales otorgados al miembro</summary>
    public int? PointsAwardedPerMember { get; set; }

    /// <summary>Clasificación de visitante</summary>
    public string? VisitorClass { get; set; }

    /// <summary>ID del miembro</summary>
    public int? MemberId { get; set; }

    /// <summary>ID del vehículo</summary>
    public int? VehicleId { get; set; }
}
