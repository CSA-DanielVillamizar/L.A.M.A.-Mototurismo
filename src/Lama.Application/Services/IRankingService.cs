using Lama.Domain.Entities;

namespace Lama.Application.Services;

/// <summary>
/// Evento generado cuando una asistencia es confirmada
/// Usado para actualización incremental de ranking
/// </summary>
public class AttendanceConfirmedEvent
{
    /// <summary>ID de la asistencia confirmada</summary>
    public int AttendanceId { get; set; }

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>ID del evento</summary>
    public int EventId { get; set; }

    /// <summary>Año del ranking</summary>
    public int Year { get; set; }

    /// <summary>Puntos otorgados en este evento</summary>
    public int PointsAwarded { get; set; }

    /// <summary>Millas recorridas en este evento</summary>
    public double MilesRecorded { get; set; }

    /// <summary>Tipo de ámbito (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</summary>
    public string ScopeType { get; set; } = string.Empty;

    /// <summary>ID del ámbito</summary>
    public string ScopeId { get; set; } = string.Empty;

    /// <summary>Clasificación de visitante</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha de confirmación</summary>
    public DateTime ConfirmedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DTO para respuesta de actualización incremental de ranking
/// </summary>
public class RankingUpdateResult
{
    /// <summary>Indica si la actualización fue exitosa</summary>
    public bool Success { get; set; }

    /// <summary>Mensaje de resultado</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Posición actual en el ranking (si aplica)</summary>
    public int? CurrentRank { get; set; }

    /// <summary>Puntos totales después de la actualización</summary>
    public int TotalPoints { get; set; }

    /// <summary>Millas totales después de la actualización</summary>
    public double TotalMiles { get; set; }
}

/// <summary>
/// DTO para respuesta de rebuild completo de ranking
/// </summary>
public class RankingRebuildResult
{
    /// <summary>Indica si el rebuild fue exitoso</summary>
    public bool Success { get; set; }

    /// <summary>Mensaje de resultado</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Número de snapshots actualizados</summary>
    public int UpdatedCount { get; set; }

    /// <summary>Fecha de inicio del rebuild</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>Fecha de finalización del rebuild</summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>Tiempo total en milisegundos</summary>
    public long ElapsedMilliseconds { get; set; }
}

/// <summary>
/// Interfaz del servicio de ranking escalable
/// Gestiona snapshots denormalizados para consultas rápidas
/// </summary>
public interface IRankingService
{
    /// <summary>
    /// Actualiza incrementalmente el ranking cuando una asistencia es confirmada
    /// Operación rápida: O(1) para cada miembro en cada ámbito
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="attendanceEvent">Evento de asistencia confirmada</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de actualización con nueva posición y puntos</returns>
    Task<RankingUpdateResult> UpdateIncrementalAsync(
        Guid tenantId,
        AttendanceConfirmedEvent attendanceEvent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rebuild completo del ranking para un año y ámbito específico
    /// Operación costosa: recalcula todos los puntos desde Attendance
    /// Recomendado ejecutar nocturnamente (Hangfire/WebJobs)
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="year">Año a recalcular</param>
    /// <param name="scopeType">Tipo de ámbito (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</param>
    /// <param name="scopeId">ID del ámbito (null para GLOBAL)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de rebuild con cantidad de registros actualizados</returns>
    Task<RankingRebuildResult> RebuildAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string? scopeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el ranking de un ámbito específico con paginación
    /// Operación rápida: consulta directa a RankingSnapshot
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="year">Año</param>
    /// <param name="scopeType">Tipo de ámbito</param>
    /// <param name="scopeId">ID del ámbito</param>
    /// <param name="skip">Número de registros a saltar (para paginación)</param>
    /// <param name="take">Número de registros a devolver (máx 100)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de snapshots ordenados por puntos (DESC)</returns>
    Task<List<RankingSnapshot>> GetRankingAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string scopeId,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el snapshot de un miembro específico en un ranking
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="year">Año</param>
    /// <param name="scopeType">Tipo de ámbito</param>
    /// <param name="scopeId">ID del ámbito</param>
    /// <param name="memberId">ID del miembro</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Snapshot del miembro o null si no existe</returns>
    Task<RankingSnapshot?> GetMemberRankingAsync(
        Guid tenantId,
        int year,
        string scopeType,
        string scopeId,
        int memberId,
        CancellationToken cancellationToken = default);
}
