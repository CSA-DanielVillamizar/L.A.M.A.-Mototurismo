using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Evento de mototurismo (viaje/rally)
/// </summary>
public class Event
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy). Default: LAMA_DEFAULT (00000000-0000-0000-0000-000000000001)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>ID del capítulo organizador</summary>
    public int ChapterId { get; set; }

    /// <summary>Número de orden del evento</summary>
    public int Order { get; set; }

    /// <summary>Fecha de inicio del evento (AAAA/MM/DD)</summary>
    public DateOnly EventStartDate { get; set; }

    /// <summary>Nombre del evento</summary>
    public required string NameOfTheEvent { get; set; }

    /// <summary>Clase del evento (1-5, define puntuación base)</summary>
    public int Class { get; set; }

    /// <summary>Distancia de ida en millas</summary>
    public double Mileage { get; set; }

    /// <summary>Puntos otorgados por evento</summary>
    public int PointsPerEvent { get; set; }

    /// <summary>Puntos otorgados por distancia recorrida</summary>
    public int PointsPerDistance { get; set; }

    /// <summary>Puntos totales otorgados por miembro</summary>
    public int PointsAwardedPerMember { get; set; }

    /// <summary>País de localidad de salida (para bonus visitante)</summary>
    public string? StartLocationCountry { get; set; }

    /// <summary>Continente de salida (para bonus visitante)</summary>
    public string? StartLocationContinent { get; set; }

    /// <summary>País de localidad de destino (para bonus visitante)</summary>
    public string? EndLocationCountry { get; set; }

    /// <summary>Continente de destino (para bonus visitante)</summary>
    public string? EndLocationContinent { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Última fecha de actualización</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Relación: Capítulo organizador</summary>
    public Chapter? Chapter { get; set; }

    /// <summary>Relación: Asistencias al evento</summary>
    public ICollection<Attendance> Attendances { get; set; } = [];
}
