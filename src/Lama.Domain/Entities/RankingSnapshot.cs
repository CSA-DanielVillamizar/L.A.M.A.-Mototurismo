using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Snapshot denormalizado del ranking de miembros por período y ámbito
/// Permite consultas rápidas sin cálculos costosos en tiempo real
/// </summary>
public class RankingSnapshot
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy). Default: LAMA_DEFAULT (00000000-0000-0000-0000-000000000001)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>Año del ranking (ej. 2026)</summary>
    public int Year { get; set; }

    /// <summary>Tipo de ámbito (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</summary>
    public string ScopeType { get; set; } = "GLOBAL";

    /// <summary>ID del ámbito (capitulo ID, país código ISO, continente, o "GLOBAL")</summary>
    public string ScopeId { get; set; } = "GLOBAL";

    /// <summary>ID del miembro en el ranking</summary>
    public int MemberId { get; set; }

    /// <summary>Navegación al miembro</summary>
    public Member? Member { get; set; }

    /// <summary>Posición en el ranking dentro del ámbito (1, 2, 3...)</summary>
    public int? Rank { get; set; }

    /// <summary>Puntos totales acumulados en el período</summary>
    public int TotalPoints { get; set; }

    /// <summary>Millas totales recorridas en el período</summary>
    public double TotalMiles { get; set; }

    /// <summary>Número de eventos completados</summary>
    public int EventsCount { get; set; }

    /// <summary>Clasificación de visitante (LOCAL, VISITOR_A, VISITOR_B, etc.)</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha y hora del último cálculo de ranking</summary>
    public DateTime LastCalculatedAt { get; set; }

    /// <summary>Fecha de creación del snapshot</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Última fecha de actualización del snapshot</summary>
    public DateTime UpdatedAt { get; set; }
}
