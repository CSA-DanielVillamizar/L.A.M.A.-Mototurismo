namespace Lama.API.Models.Ranking;

/// <summary>
/// DTO para un item individual en la lista de ranking
/// </summary>
public class RankingItemDto
{
    /// <summary>Posición en el ranking</summary>
    public int? Rank { get; set; }

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public string? MemberName { get; set; }

    /// <summary>Puntos totales acumulados</summary>
    public int TotalPoints { get; set; }

    /// <summary>Millas totales recorridas</summary>
    public double TotalMiles { get; set; }

    /// <summary>Número de eventos completados</summary>
    public int EventsCount { get; set; }

    /// <summary>Clasificación de visitante (LOCAL, VISITOR_A, VISITOR_B)</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha del último cálculo</summary>
    public DateTime LastCalculatedAt { get; set; }
}

/// <summary>
/// DTO para respuesta de lista de ranking con metadatos
/// </summary>
public class RankingListResponseDto
{
    /// <summary>Año del ranking</summary>
    public int Year { get; set; }

    /// <summary>Tipo de ámbito (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</summary>
    public string ScopeType { get; set; } = string.Empty;

    /// <summary>ID del ámbito</summary>
    public string ScopeId { get; set; } = string.Empty;

    /// <summary>Total de miembros en este ranking</summary>
    public int TotalCount { get; set; }

    /// <summary>Número de miembros en esta página</summary>
    public int PageCount { get; set; }

    /// <summary>Página actual (skip/take)</summary>
    public int PageNumber { get; set; }

    /// <summary>Items por página</summary>
    public int PageSize { get; set; }

    /// <summary>Items del ranking</summary>
    public List<RankingItemDto> Items { get; set; } = new();

    /// <summary>Fecha de última actualización del ranking</summary>
    public DateTime? LastUpdatedAt { get; set; }
}

/// <summary>
/// DTO para respuesta de miembro en su ranking específico
/// </summary>
public class MemberRankingDto
{
    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public string? MemberName { get; set; }

    /// <summary>Año del ranking</summary>
    public int Year { get; set; }

    /// <summary>Tipo de ámbito</summary>
    public string ScopeType { get; set; } = string.Empty;

    /// <summary>ID del ámbito</summary>
    public string ScopeId { get; set; } = string.Empty;

    /// <summary>Posición en el ranking</summary>
    public int? Rank { get; set; }

    /// <summary>Total de miembros en este ranking</summary>
    public int TotalMembersInScope { get; set; }

    /// <summary>Puntos totales acumulados</summary>
    public int TotalPoints { get; set; }

    /// <summary>Millas totales recorridas</summary>
    public double TotalMiles { get; set; }

    /// <summary>Número de eventos completados</summary>
    public int EventsCount { get; set; }

    /// <summary>Promedio de puntos por evento</summary>
    public decimal AveragePointsPerEvent => EventsCount > 0 ? TotalPoints / (decimal)EventsCount : 0;

    /// <summary>Promedio de millas por evento</summary>
    public decimal AverageMilesPerEvent => EventsCount > 0 ? (decimal)TotalMiles / EventsCount : 0;

    /// <summary>Clasificación de visitante</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha del último cálculo</summary>
    public DateTime LastCalculatedAt { get; set; }

    /// <summary>Progreso: posición relativa de 0-100</summary>
    public decimal ProgressPercentage => TotalMembersInScope > 0 
        ? ((TotalMembersInScope - (Rank ?? TotalMembersInScope)) / (decimal)TotalMembersInScope) * 100 
        : 0;
}

/// <summary>
/// DTO para respuesta de dashboard del miembro
/// Combina información de ranking + estadísticas personales
/// </summary>
public class MemberDashboardDto
{
    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public string? MemberName { get; set; }

    /// <summary>Año actual</summary>
    public int CurrentYear { get; set; }

    /// <summary>Rankings del miembro por diferentes ámbitos</summary>
    public List<RankingByScope> Rankings { get; set; } = new();

    /// <summary>Estadísticas generales del período</summary>
    public MemberStatsDto Stats { get; set; } = new();
}

/// <summary>
/// Sub-DTO: ranking de un miembro en un ámbito específico
/// </summary>
public class RankingByScope
{
    /// <summary>Tipo de ámbito (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</summary>
    public string ScopeType { get; set; } = string.Empty;

    /// <summary>ID del ámbito</summary>
    public string ScopeId { get; set; } = string.Empty;

    /// <summary>Nombre del ámbito para mostrar (ej: "Colombia", "Americas")</summary>
    public string ScopeName { get; set; } = string.Empty;

    /// <summary>Posición en el ranking</summary>
    public int? Rank { get; set; }

    /// <summary>Total de miembros en este ámbito</summary>
    public int TotalMembers { get; set; }

    /// <summary>Puntos en este ámbito</summary>
    public int Points { get; set; }

    /// <summary>Porcentaje de progreso: qué tan cerca del #1</summary>
    public decimal ProgressPercentage { get; set; }
}

/// <summary>
/// Sub-DTO: estadísticas del miembro en el período
/// </summary>
public class MemberStatsDto
{
    /// <summary>Puntos totales acumulados</summary>
    public int TotalPoints { get; set; }

    /// <summary>Millas totales recorridas</summary>
    public double TotalMiles { get; set; }

    /// <summary>Número de eventos completados</summary>
    public int EventsCount { get; set; }

    /// <summary>Promedio de puntos por evento</summary>
    public decimal AveragePointsPerEvent { get; set; }

    /// <summary>Promedio de millas por evento</summary>
    public decimal AverageMilesPerEvent { get; set; }

    /// <summary>Próximo hito de puntos</summary>
    public int NextMilestonePoints { get; set; }

    /// <summary>Puntos faltantes para próximo hito</summary>
    public int PointsUntilNextMilestone { get; set; }
}
