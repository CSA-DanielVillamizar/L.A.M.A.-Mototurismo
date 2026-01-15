using Lama.Application.Abstractions;
using Lama.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Servicio escalable de ranking con snapshots denormalizados
/// Combina actualizaciones incrementales (rápidas) y rebuilds completos (nocturnos)
/// </summary>
public class RankingService : IRankingService
{
    private readonly ILamaDbContext _dbContext;
    private readonly ILogger<RankingService> _logger;

    public RankingService(
        ILamaDbContext dbContext,
        ILogger<RankingService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Actualización incremental: suma puntos al snapshot existente
    /// Estrategia MVP: ejecutada inmediatamente al aprobar evidencia
    /// </summary>
    public async Task<RankingUpdateResult> UpdateIncrementalAsync(
        Guid tenantId,
        AttendanceConfirmedEvent attendanceEvent,
        CancellationToken cancellationToken = default)
    {
        var result = new RankingUpdateResult();

        try
        {
            _logger.LogInformation(
                "Iniciando actualización incremental de ranking. TenantId: {TenantId}, MemberId: {MemberId}, Year: {Year}, EventId: {EventId}",
                tenantId, attendanceEvent.MemberId, attendanceEvent.Year, attendanceEvent.EventId);

            // 1. Obtener o crear snapshot
            var snapshot = await _dbContext.RankingSnapshots
                .FirstOrDefaultAsync(
                    rs => rs.TenantId == tenantId &&
                          rs.Year == attendanceEvent.Year &&
                          rs.ScopeType == attendanceEvent.ScopeType &&
                          rs.ScopeId == attendanceEvent.ScopeId &&
                          rs.MemberId == attendanceEvent.MemberId,
                    cancellationToken);

            if (snapshot == null)
            {
                // Crear nuevo snapshot
                snapshot = new Lama.Domain.Entities.RankingSnapshot
                {
                    TenantId = tenantId,
                    Year = attendanceEvent.Year,
                    ScopeType = attendanceEvent.ScopeType,
                    ScopeId = attendanceEvent.ScopeId,
                    MemberId = attendanceEvent.MemberId,
                    TotalPoints = attendanceEvent.PointsAwarded,
                    TotalMiles = attendanceEvent.MilesRecorded,
                    EventsCount = 1,
                    VisitorClass = attendanceEvent.VisitorClass,
                    LastCalculatedAt = attendanceEvent.ConfirmedAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.RankingSnapshots.Add(snapshot);
                _logger.LogInformation("Nuevo snapshot creado para MemberId: {MemberId}", attendanceEvent.MemberId);
            }
            else
            {
                // Actualizar snapshot existente
                snapshot.TotalPoints += attendanceEvent.PointsAwarded;
                snapshot.TotalMiles += attendanceEvent.MilesRecorded;
                snapshot.EventsCount += 1;
                snapshot.VisitorClass = attendanceEvent.VisitorClass ?? snapshot.VisitorClass;
                snapshot.LastCalculatedAt = attendanceEvent.ConfirmedAt;
                snapshot.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation(
                    "Snapshot actualizado. MemberId: {MemberId}, NewTotalPoints: {NewTotalPoints}, NewEventsCount: {NewEventsCount}",
                    attendanceEvent.MemberId, snapshot.TotalPoints, snapshot.EventsCount);
            }

            // 2. Guardar cambios
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 3. Calcular nueva posición (rango)
            var newRank = await CalculateRankAsync(
                tenantId,
                attendanceEvent.Year,
                attendanceEvent.ScopeType,
                attendanceEvent.ScopeId,
                attendanceEvent.MemberId,
                cancellationToken);

            snapshot.Rank = newRank;
            await _dbContext.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Message = $"Ranking actualizado exitosamente. Nuevos puntos: {snapshot.TotalPoints}, Posición: {newRank}";
            result.CurrentRank = newRank;
            result.TotalPoints = snapshot.TotalPoints;
            result.TotalMiles = snapshot.TotalMiles;

            _logger.LogInformation("Actualización incremental completada. MemberId: {MemberId}, Rank: {Rank}",
                attendanceEvent.MemberId, newRank);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización incremental de ranking. MemberId: {MemberId}",
                attendanceEvent.MemberId);

            result.Success = false;
            result.Message = $"Error actualizando ranking: {ex.Message}";
            return result;
        }
    }

    /// <summary>
    /// Rebuild completo: recalcula todos los rankings desde cero
    /// Operación costosa pero precisa. Ejecutar nocturnamente.
    /// </summary>
    public async Task<RankingRebuildResult> RebuildAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string? scopeId = null,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = new RankingRebuildResult { StartedAt = startTime };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Iniciando rebuild completo de ranking. TenantId: {TenantId}, Year: {Year}, ScopeType: {ScopeType}, ScopeId: {ScopeId}",
                tenantId, year, scopeType, scopeId);

            // 1. Obtener todos los Attendance confirmados en el período
            var attendances = await _dbContext.Attendance
                .Include(a => a.Member)
                .Where(a => a.TenantId == tenantId &&
                           a.Status == "CONFIRMED" &&
                           a.Event!.EventStartDate.Year == year)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Se encontraron {Count} asistencias confirmadas para rebuild", attendances.Count);

            // 2. Agrupar y calcular puntos por miembro y ámbito
            var rankingData = new Dictionary<string, Dictionary<int, RankingAggregateData>>();

            foreach (var attendance in attendances)
            {
                var key = $"{scopeType}:{scopeId ?? "GLOBAL"}";

                if (!rankingData.ContainsKey(key))
                {
                    rankingData[key] = new Dictionary<int, RankingAggregateData>();
                }

                if (!rankingData[key].ContainsKey(attendance.MemberId))
                {
                    rankingData[key][attendance.MemberId] = new RankingAggregateData
                    {
                        MemberId = attendance.MemberId,
                        TotalPoints = 0,
                        TotalMiles = 0,
                        EventsCount = 0
                    };
                }

                var memberData = rankingData[key][attendance.MemberId];
                memberData.TotalPoints += attendance.PointsAwardedPerMember ?? 0;
                memberData.TotalMiles += attendance.Event?.Mileage ?? 0;
                memberData.EventsCount += 1;
                memberData.VisitorClass = attendance.VisitorClass;
            }

            // 3. Eliminar snapshots antiguos del período
            var oldSnapshots = await _dbContext.RankingSnapshots
                .Where(rs => rs.TenantId == tenantId &&
                            rs.Year == year &&
                            rs.ScopeType == scopeType &&
                            (scopeId == null ? rs.ScopeId == "GLOBAL" : rs.ScopeId == scopeId))
                .ToListAsync(cancellationToken);

            _dbContext.RankingSnapshots.RemoveRange(oldSnapshots);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Se eliminaron {Count} snapshots antiguos", oldSnapshots.Count);

            // 4. Crear nuevos snapshots ordenados por puntos
            var newSnapshots = new List<Lama.Domain.Entities.RankingSnapshot>();
            int updatedCount = 0;

            foreach (var scopeKey in rankingData.Keys)
            {
                var parts = scopeKey.Split(':');
                var members = rankingData[scopeKey]
                    .OrderByDescending(x => x.Value.TotalPoints)
                    .ToList();

                int rank = 1;
                foreach (var memberEntry in members)
                {
                    var memberData = memberEntry.Value;

                    var snapshot = new Lama.Domain.Entities.RankingSnapshot
                    {
                        TenantId = tenantId,
                        Year = year,
                        ScopeType = scopeType,
                        ScopeId = scopeId ?? "GLOBAL",
                        MemberId = memberData.MemberId,
                        Rank = rank,
                        TotalPoints = memberData.TotalPoints,
                        TotalMiles = memberData.TotalMiles,
                        EventsCount = memberData.EventsCount,
                        VisitorClass = memberData.VisitorClass,
                        LastCalculatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    newSnapshots.Add(snapshot);
                    rank++;
                    updatedCount++;
                }
            }

            // 5. Insertar nuevos snapshots
            if (newSnapshots.Count > 0)
            {
                _dbContext.RankingSnapshots.AddRange(newSnapshots);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Se crearon {Count} nuevos snapshots", newSnapshots.Count);
            }

            stopwatch.Stop();

            result.Success = true;
            result.Message = $"Rebuild completado exitosamente. {updatedCount} miembros actualizados.";
            result.UpdatedCount = updatedCount;
            result.CompletedAt = DateTime.UtcNow;
            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "Rebuild completo finalizado. Year: {Year}, ScopeType: {ScopeType}, UpdatedCount: {UpdatedCount}, ElapsedMs: {ElapsedMs}",
                year, scopeType, updatedCount, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "Error en rebuild de ranking. Year: {Year}, ScopeType: {ScopeType}",
                year, scopeType);

            result.Success = false;
            result.Message = $"Error en rebuild: {ex.Message}";
            result.CompletedAt = DateTime.UtcNow;
            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            return result;
        }
    }

    /// <summary>
    /// Obtiene el ranking ordenado por puntos con paginación
    /// </summary>
    public async Task<List<Lama.Domain.Entities.RankingSnapshot>> GetRankingAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string scopeId,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        // Limitar take a máximo 100 por seguridad de performance
        take = Math.Min(take, 100);
        skip = Math.Max(skip, 0);

        _logger.LogInformation(
            "Consultando ranking. Year: {Year}, ScopeType: {ScopeType}, ScopeId: {ScopeId}, Skip: {Skip}, Take: {Take}",
            year, scopeType, scopeId, skip, take);

        var snapshots = await _dbContext.RankingSnapshots
            .Where(rs => rs.TenantId == tenantId &&
                        rs.Year == year &&
                        rs.ScopeType == scopeType &&
                        rs.ScopeId == scopeId)
            .Include(rs => rs.Member)
            .OrderByDescending(rs => rs.TotalPoints)
            .ThenByDescending(rs => rs.TotalMiles)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return snapshots;
    }

    /// <summary>
    /// Obtiene el ranking de un miembro específico
    /// </summary>
    public async Task<Lama.Domain.Entities.RankingSnapshot?> GetMemberRankingAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string scopeId,
        int memberId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await _dbContext.RankingSnapshots
            .Include(rs => rs.Member)
            .FirstOrDefaultAsync(
                rs => rs.TenantId == tenantId &&
                      rs.Year == year &&
                      rs.ScopeType == scopeType &&
                      rs.ScopeId == scopeId &&
                      rs.MemberId == memberId,
                cancellationToken);

        return snapshot;
    }

    /// <summary>
    /// Calcula la posición (rank) de un miembro en su ámbito
    /// Contadores descendentes: mayores puntos = mejor rank
    /// </summary>
    private async Task<int> CalculateRankAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string scopeId,
        int memberId,
        CancellationToken cancellationToken)
    {
        var currentSnapshot = await _dbContext.RankingSnapshots
            .FirstOrDefaultAsync(
                rs => rs.TenantId == tenantId &&
                      rs.Year == year &&
                      rs.ScopeType == scopeType &&
                      rs.ScopeId == scopeId &&
                      rs.MemberId == memberId,
                cancellationToken);

        if (currentSnapshot == null)
            return 1;

        // Contar cuántos miembros tienen más puntos
        var betterRanked = await _dbContext.RankingSnapshots
            .CountAsync(
                rs => rs.TenantId == tenantId &&
                      rs.Year == year &&
                      rs.ScopeType == scopeType &&
                      rs.ScopeId == scopeId &&
                      rs.TotalPoints > currentSnapshot.TotalPoints,
                cancellationToken);

        return betterRanked + 1;
    }

    /// <summary>
    /// DTO interno para agregación de datos de ranking durante rebuild
    /// </summary>
    private class RankingAggregateData
    {
        public int MemberId { get; set; }
        public int TotalPoints { get; set; }
        public double TotalMiles { get; set; }
        public int EventsCount { get; set; }
        public string? VisitorClass { get; set; }
    }
}
