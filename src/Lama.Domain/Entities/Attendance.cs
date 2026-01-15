using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Registro de asistencia de un miembro a un evento
/// </summary>
public class Attendance
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy). Default: LAMA_DEFAULT (00000000-0000-0000-0000-000000000001)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>ID del evento</summary>
    public int EventId { get; set; }

    /// <summary>ID del miembro asistente</summary>
    public int MemberId { get; set; }

    /// <summary>ID del vehículo usado en el evento</summary>
    public int VehicleId { get; set; }

    /// <summary>Estado de la asistencia (PENDING, CONFIRMED, REJECTED)</summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>Puntos otorgados por el evento</summary>
    public int? PointsPerEvent { get; set; }

    /// <summary>Puntos otorgados por distancia</summary>
    public int? PointsPerDistance { get; set; }

    /// <summary>Puntos totales otorgados al miembro</summary>
    public int? PointsAwardedPerMember { get; set; }

    /// <summary>Clasificación de visitante (LOCAL, VISITOR_A, VISITOR_B)</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha y hora de confirmación</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>ID del MTO/Admin que confirmó la asistencia</summary>
    public int? ConfirmedBy { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Última fecha de actualización</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Relación: Evento</summary>
    public Event? Event { get; set; }

    /// <summary>Relación: Miembro</summary>
    public Member? Member { get; set; }

    /// <summary>Relación: Vehículo</summary>
    public Vehicle? Vehicle { get; set; }

    /// <summary>Relación: Confirmador</summary>
    public Member? ConfirmedByMember { get; set; }
}
