using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Miembro de LAMA (piloto de moto)
/// </summary>
public class Member
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del capítulo al que pertenece</summary>
    public int ChapterId { get; set; }

    /// <summary>Número de orden en el registro</summary>
    public int Order { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public required string CompleteNames { get; set; }

    /// <summary>Indica si es dama (género femenino)</summary>
    public bool Dama { get; set; }

    /// <summary>País de nacimiento</summary>
    public string? CountryBirth { get; set; }

    /// <summary>Año de ingreso a LAMA</summary>
    public int? InLamaSince { get; set; }

    /// <summary>Estado del miembro (uno de los 33 valores del dropdown de MemberStatusTypes)</summary>
    public required string Status { get; set; }

    /// <summary>Indica si el miembro es elegible para campeonato</summary>
    public bool IsEligible { get; set; }

    /// <summary>Continente (Africa, Americas, Asia, Europe, Oceania)</summary>
    public string? Continent { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Última fecha de actualización</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Relación: Capítulo al que pertenece</summary>
    public Chapter? Chapter { get; set; }

    /// <summary>Relación: Vehículos del miembro</summary>
    public ICollection<Vehicle> Vehicles { get; set; } = [];

    /// <summary>Relación: Asistencias del miembro a eventos</summary>
    public ICollection<Attendance> Attendances { get; set; } = [];
}
