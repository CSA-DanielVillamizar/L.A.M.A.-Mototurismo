using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lama.API.Models.Ranking;
using Lama.API.Utilities;
using Lama.Application.Abstractions;
using Lama.Application.Services;
using Lama.Domain.Entities;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para consultas de ranking escalable basado en snapshots
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RankingsController : ControllerBase
{
    private readonly ILamaDbContext _dbContext;
    private readonly IRankingService _rankingService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<RankingsController> _logger;

    public RankingsController(
        ILamaDbContext dbContext,
        IRankingService rankingService,
        ITenantProvider tenantProvider,
        ILogger<RankingsController> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene el ranking de un período y ámbito específico con paginación
    /// Consulta rápida desde RankingSnapshot (denormalizado)
    /// </summary>
    /// <param name="year">Año (ej. 2026)</param>
    /// <param name="scopeType">Tipo de ámbito: CHAPTER, COUNTRY, CONTINENT, GLOBAL</param>
    /// <param name="scopeId">ID del ámbito (código ISO para COUNTRY, código para CHAPTER, continente para CONTINENT, "GLOBAL" para GLOBAL)</param>
    /// <param name="skip">Número de registros a saltar (paginación)</param>
    /// <param name="take">Número de registros a devolver (máximo 100)</param>
    /// <returns>Lista de miembros en ranking ordenados por puntos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(RankingListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RankingListResponseDto>> GetRankingAsync(
        [FromQuery] int year,
        [FromQuery] string scopeType = "GLOBAL",
        [FromQuery] string? scopeId = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100)
    {
        try
        {
            // Validaciones
            if (year < 2000 || year > DateTime.UtcNow.Year + 10)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid year",
                    detail: "Año inválido");
            }

            var validScopes = new[] { "CHAPTER", "COUNTRY", "CONTINENT", "GLOBAL" };
            if (!validScopes.Contains(scopeType, StringComparer.OrdinalIgnoreCase))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid scope type",
                    detail: $"ScopeType debe ser uno de: {string.Join(", ", validScopes)}");
            }

            scopeType = scopeType.ToUpper();
            scopeId = scopeId?.ToUpper() ?? "GLOBAL";

            // Limitar paginación
            skip = Math.Max(skip, 0);
            take = Math.Min(Math.Max(take, 1), 100);

            _logger.LogInformation(
                "Consultando ranking. Year: {Year}, ScopeType: {ScopeType}, ScopeId: {ScopeId}, Skip: {Skip}, Take: {Take}",
                year, scopeType, scopeId, skip, take);

            // Obtener rankings desde snapshot
            var snapshots = await _rankingService.GetRankingAsync(
                _tenantProvider.CurrentTenantId,
                year,
                scopeType,
                scopeId,
                skip,
                take);

            // Contar total en este ranking
            var totalCount = await _dbContext.RankingSnapshots
                .CountAsync(rs => rs.TenantId == _tenantProvider.CurrentTenantId &&
                                  rs.Year == year &&
                                  rs.ScopeType == scopeType &&
                                  rs.ScopeId == scopeId);

            // Mapear a DTOs
            var items = snapshots.Select(rs => new RankingItemDto
            {
                Rank = rs.Rank,
                MemberId = rs.MemberId,
                MemberName = rs.Member?.CompleteNames ?? "N/A",
                TotalPoints = rs.TotalPoints,
                TotalMiles = rs.TotalMiles,
                EventsCount = rs.EventsCount,
                VisitorClass = rs.VisitorClass,
                LastCalculatedAt = rs.LastCalculatedAt
            }).ToList();

            var lastUpdated = snapshots.FirstOrDefault()?.LastCalculatedAt;

            var response = new RankingListResponseDto
            {
                Year = year,
                ScopeType = scopeType,
                ScopeId = scopeId,
                TotalCount = totalCount,
                PageCount = items.Count,
                PageNumber = skip / take + 1,
                PageSize = take,
                Items = items,
                LastUpdatedAt = lastUpdated
            };

            _logger.LogInformation(
                "Ranking consultado exitosamente. Year: {Year}, ScopeType: {ScopeType}, TotalCount: {TotalCount}",
                year, scopeType, totalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar ranking");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Ranking retrieval failure",
                detail: "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el ranking de un miembro específico en diferentes ámbitos
    /// Usado en dashboard del miembro para mostrar su progreso
    /// </summary>
    /// <param name="memberId">ID del miembro</param>
    /// <param name="year">Año (opcional, por defecto año actual)</param>
    /// <returns>Dashboard con rankings del miembro en todos los ámbitos</returns>
    [HttpGet("member/{memberId}/dashboard")]
    [ProducesResponseType(typeof(MemberDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDashboardDto>> GetMemberDashboardAsync(
        int memberId,
        [FromQuery] int? year = null)
    {
        try
        {
            year = year ?? DateTime.UtcNow.Year;

            // Validar que el miembro existe
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.Id == memberId && m.TenantId == _tenantProvider.CurrentTenantId);

            if (member == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Member not found",
                    detail: $"Miembro {memberId} no encontrado");
            }

            _logger.LogInformation("Obteniendo dashboard para MemberId: {MemberId}, Year: {Year}",
                memberId, year);

            // Obtener todos los rankings del miembro en diferentes ámbitos
            var rankingSnapshots = await _dbContext.RankingSnapshots
                .Where(rs => rs.TenantId == _tenantProvider.CurrentTenantId &&
                            rs.Year == year &&
                            rs.MemberId == memberId)
                .ToListAsync();

            if (rankingSnapshots.Count == 0)
            {
                // Si no hay snapshots, crear respuesta vacía
                return Ok(new MemberDashboardDto
                {
                    MemberId = memberId,
                    MemberName = member.CompleteNames,
                    CurrentYear = year.Value,
                    Rankings = new(),
                    Stats = new()
                });
            }

            // Agrupar por ScopeType y obtener el snapshot de cada ámbito
            var rankingsByScope = new List<RankingByScope>();

            foreach (var snapshot in rankingSnapshots)
            {
                // Obtener total de miembros en este ámbito
                var totalMembers = await _dbContext.RankingSnapshots
                    .CountAsync(rs => rs.TenantId == _tenantProvider.CurrentTenantId &&
                                      rs.Year == year &&
                                      rs.ScopeType == snapshot.ScopeType &&
                                      rs.ScopeId == snapshot.ScopeId);

                // Calcular progreso: qué tan lejos del #1
                var topPoints = await _dbContext.RankingSnapshots
                    .Where(rs => rs.TenantId == _tenantProvider.CurrentTenantId &&
                                rs.Year == year &&
                                rs.ScopeType == snapshot.ScopeType &&
                                rs.ScopeId == snapshot.ScopeId)
                    .OrderByDescending(rs => rs.TotalPoints)
                    .Select(rs => rs.TotalPoints)
                    .FirstOrDefaultAsync();

                var progressPercentage = topPoints > 0
                    ? (snapshot.TotalPoints / (decimal)topPoints) * 100m
                    : 0m;

                var scopeName = GetScopeName(snapshot.ScopeType, snapshot.ScopeId);

                rankingsByScope.Add(new RankingByScope
                {
                    ScopeType = snapshot.ScopeType,
                    ScopeId = snapshot.ScopeId,
                    ScopeName = scopeName,
                    Rank = snapshot.Rank,
                    TotalMembers = totalMembers,
                    Points = snapshot.TotalPoints,
                    ProgressPercentage = Math.Round(progressPercentage, 2)
                });
            }

            // Calcular estadísticas generales
            var globalRanking = rankingSnapshots.FirstOrDefault(rs => rs.ScopeType == "GLOBAL");

            var stats = new MemberStatsDto
            {
                TotalPoints = globalRanking?.TotalPoints ?? 0,
                TotalMiles = globalRanking?.TotalMiles ?? 0,
                EventsCount = globalRanking?.EventsCount ?? 0,
                AveragePointsPerEvent = (globalRanking?.EventsCount ?? 0) > 0
                    ? (globalRanking!.TotalPoints / (decimal)(globalRanking.EventsCount))
                    : 0,
                AverageMilesPerEvent = (globalRanking?.EventsCount ?? 0) > 0
                    ? (decimal)(globalRanking!.TotalMiles / globalRanking.EventsCount)
                    : 0,
                NextMilestonePoints = GetNextMilestone(globalRanking?.TotalPoints ?? 0),
                PointsUntilNextMilestone = GetNextMilestone(globalRanking?.TotalPoints ?? 0) - (globalRanking?.TotalPoints ?? 0)
            };

            var response = new MemberDashboardDto
            {
                MemberId = memberId,
                MemberName = member.CompleteNames,
                CurrentYear = year.Value,
                Rankings = rankingsByScope.OrderBy(r => r.Rank).ToList(),
                Stats = stats
            };

            _logger.LogInformation(
                "Dashboard obtenido exitosamente. MemberId: {MemberId}, Rankings: {RankingCount}",
                memberId, rankingsByScope.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard del miembro");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Member dashboard failure",
                detail: "Error interno del servidor");
        }
    }

    /// <summary>
    /// Endpoint administrativo para ejecutar rebuild de ranking
    /// Operación costosa: recalcula todos los puntos desde Attendance
    /// </summary>
    /// <param name="year">Año a recalcular</param>
    /// <param name="scopeType">Tipo de ámbito a recalcular</param>
    /// <param name="scopeId">ID del ámbito (opcional)</param>
    /// <returns>Resultado del rebuild con estadísticas</returns>
    [HttpPost("rebuild")]
    [Authorize(Policy = "IsSuperAdmin")]
    [ProducesResponseType(typeof(RankingRebuildResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RankingRebuildResult>> RebuildRankingAsync(
        [FromQuery] int year,
        [FromQuery] string scopeType = "GLOBAL",
        [FromQuery] string? scopeId = null)
    {
        try
        {
            if (year < 2000 || year > DateTime.UtcNow.Year + 10)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid year",
                    detail: "Año inválido");
            }

            scopeType = scopeType.ToUpper();
            scopeId = scopeId?.ToUpper();

            _logger.LogInformation(
                "Iniciando rebuild de ranking. Year: {Year}, ScopeType: {ScopeType}, ScopeId: {ScopeId}",
                year, scopeType, scopeId);

            var result = await _rankingService.RebuildAsync(
                _tenantProvider.CurrentTenantId,
                year,
                scopeType,
                scopeId);

            _logger.LogInformation(
                "Rebuild completado. Year: {Year}, ScopeType: {ScopeType}, UpdatedCount: {UpdatedCount}, ElapsedMs: {ElapsedMs}",
                year, scopeType, result.UpdatedCount, result.ElapsedMilliseconds);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en rebuild de ranking");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Ranking rebuild failure",
                detail: "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el ranking de un miembro en un ámbito específico
    /// Información detallada para visualización
    /// </summary>
    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(MemberRankingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberRankingDto>> GetMemberRankingAsync(
        int memberId,
        [FromQuery] string scopeType = "GLOBAL",
        [FromQuery] string? scopeId = null,
        [FromQuery] int? year = null)
    {
        try
        {
            year = year ?? DateTime.UtcNow.Year;
            scopeType = scopeType.ToUpper();
            scopeId = scopeId?.ToUpper() ?? "GLOBAL";

            // Obtener snapshot del miembro
            var snapshot = await _rankingService.GetMemberRankingAsync(
                _tenantProvider.CurrentTenantId,
                year.Value,
                scopeType,
                scopeId,
                memberId);

            if (snapshot == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Member ranking not found",
                    detail: $"Miembro {memberId} no tiene ranking en este período");
            }

            // Obtener total de miembros en el ámbito
            var totalMembers = await _dbContext.RankingSnapshots
                .CountAsync(rs => rs.TenantId == _tenantProvider.CurrentTenantId &&
                                  rs.Year == year &&
                                  rs.ScopeType == scopeType &&
                                  rs.ScopeId == scopeId);

            var dto = new MemberRankingDto
            {
                MemberId = snapshot.MemberId,
                MemberName = snapshot.Member?.CompleteNames ?? "N/A",
                Year = snapshot.Year,
                ScopeType = snapshot.ScopeType,
                ScopeId = snapshot.ScopeId,
                Rank = snapshot.Rank,
                TotalMembersInScope = totalMembers,
                TotalPoints = snapshot.TotalPoints,
                TotalMiles = snapshot.TotalMiles,
                EventsCount = snapshot.EventsCount,
                VisitorClass = snapshot.VisitorClass,
                LastCalculatedAt = snapshot.LastCalculatedAt
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ranking del miembro");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Member ranking failure",
                detail: "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el nombre legible de un ámbito
    /// </summary>
    private string GetScopeName(string scopeType, string scopeId)
    {
        return scopeType switch
        {
            "GLOBAL" => "Ranking Global",
            "CONTINENT" => $"Continente: {scopeId}",
            "COUNTRY" => $"País: {scopeId}",
            "CHAPTER" => $"Capítulo: {scopeId}",
            _ => scopeId
        };
    }

    /// <summary>
    /// Calcula el siguiente hito de puntos (ej. 100, 500, 1000, etc.)
    /// </summary>
    private int GetNextMilestone(int currentPoints)
    {
        return currentPoints switch
        {
            < 100 => 100,
            < 500 => 500,
            < 1000 => 1000,
            < 5000 => 5000,
            < 10000 => 10000,
            _ => (currentPoints / 10000 + 1) * 10000
        };
    }
}
