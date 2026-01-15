namespace Lama.Domain.Enums;

/// <summary>
/// Tipos de entidades que pueden ser auditadas.
/// </summary>
public enum AuditEntityType
{
    /// <summary>Evidencia (Evidence)</summary>
    Evidence = 1,

    /// <summary>Asistencia (Attendance)</summary>
    Attendance = 2,

    /// <summary>Vehículo (Vehicle)</summary>
    Vehicle = 3,

    /// <summary>Miembro (Member)</summary>
    Member = 4,

    /// <summary>Evento (Event)</summary>
    Event = 5,

    /// <summary>Rol de miembro (MemberRole)</summary>
    MemberRole = 6,

    /// <summary>Scope de miembro (MemberScope)</summary>
    MemberScope = 7,

    /// <summary>Configuración (Configuration)</summary>
    Configuration = 8,

    /// <summary>Capítulo (Chapter)</summary>
    Chapter = 9,

    /// <summary>País (Country)</summary>
    Country = 10,

    /// <summary>Continente (Continent)</summary>
    Continent = 11,

    /// <summary>Ranking snapshot</summary>
    RankingSnapshot = 12,

    /// <summary>Usuario (User/Principal)</summary>
    User = 13
}
