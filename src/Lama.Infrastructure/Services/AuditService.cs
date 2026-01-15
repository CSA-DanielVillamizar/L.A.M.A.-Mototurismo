using System.Text.Json;
using Lama.Application.Abstractions;
using Lama.Application.Services;
using Lama.Domain.Entities;
using Lama.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de auditoría para registrar y consultar acciones auditables.
/// </summary>
public class AuditService : IAuditService
{
    private readonly ILamaDbContext _dbContext;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILamaDbContext dbContext, ILogger<AuditService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registra una acción auditable en la base de datos.
    /// </summary>
    public async Task LogAsync(
        Guid tenantId,
        string actorExternalSubjectId,
        int? actorMemberId,
        AuditActionType action,
        AuditEntityType entityType,
        string entityId,
        string? beforeJson = null,
        string? afterJson = null,
        string? notes = null,
        string? correlationId = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                TenantId = tenantId,
                ActorExternalSubjectId = actorExternalSubjectId,
                ActorMemberId = actorMemberId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                BeforeJson = beforeJson,
                AfterJson = afterJson,
                Notes = notes,
                CorrelationId = correlationId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Audit log created: {Action} on {EntityType} {EntityId} by {ActorMemberId} (CorrelationId: {CorrelationId})",
                action, entityType, entityId, actorMemberId, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error while logging audit action: {Action} on {EntityType} {EntityId}. CorrelationId: {CorrelationId}",
                action, entityType, entityId, correlationId);
            
            // No relanzamos excepción para no detener la operación principal
            // pero sí dejamos constancia del error
        }
    }

    /// <summary>
    /// Obtiene auditorías de un miembro específico, ordenadas por fecha descendente.
    /// </summary>
    public async Task<IEnumerable<AuditLogDto>> GetAuditsByMemberAsync(Guid tenantId, int actorMemberId, int take = 100)
    {
        // Limitar a máximo 1000 registros para evitar sobrecarga
        take = Math.Min(take, 1000);

        try
        {
            var audits = await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(a => a.TenantId == tenantId && a.ActorMemberId == actorMemberId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .Select(a => MapToDto(a))
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} audit logs for member {MemberId}", audits.Count, actorMemberId);
            return audits;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for member {MemberId}", actorMemberId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene auditorías de una entidad específica, ordenadas por fecha descendente.
    /// Permite rastrear todos los cambios realizados a una entidad en particular.
    /// </summary>
    public async Task<IEnumerable<AuditLogDto>> GetAuditsByEntityAsync(
        Guid tenantId,
        AuditEntityType entityType,
        string entityId,
        int take = 100)
    {
        take = Math.Min(take, 1000);

        try
        {
            var audits = await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(a => a.TenantId == tenantId && a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .Select(a => MapToDto(a))
                .ToListAsync();

            _logger.LogInformation(
                "Retrieved {Count} audit logs for {EntityType} {EntityId}",
                audits.Count, entityType, entityId);

            return audits;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error retrieving audit logs for {EntityType} {EntityId}",
                entityType, entityId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene auditorías con un ID de correlación específico.
    /// Permite rastrear una solicitud distribuida a través de todo el sistema.
    /// </summary>
    public async Task<IEnumerable<AuditLogDto>> GetAuditsByCorrelationIdAsync(string correlationId)
    {
        try
        {
            var audits = await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(a => a.CorrelationId == correlationId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => MapToDto(a))
                .ToListAsync();

            _logger.LogInformation(
                "Retrieved {Count} audit logs for CorrelationId {CorrelationId}",
                audits.Count, correlationId);

            return audits;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for CorrelationId {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene un resumen agregado de auditoría para los últimos N días.
    /// Útil para análisis de seguridad y cumplimiento normativo.
    /// </summary>
    public async Task<AuditSummaryDto> GetAuditSummaryAsync(Guid tenantId, int days = 30)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            var audits = await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(a => a.TenantId == tenantId && a.CreatedAt >= startDate)
                .ToListAsync();

            var actionCounts = audits
                .GroupBy(a => a.Action)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            var entityTypeCounts = audits
                .GroupBy(a => a.EntityType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            var topActiveMembers = audits
                .Where(a => a.ActorMemberId.HasValue)
                .GroupBy(a => a.ActorMemberId)
                .Select(g => new { MemberId = g.Key!.Value, Count = g.Count(), LastDate = g.Max(a => a.CreatedAt) })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => new MemberActivityDto
                {
                    MemberId = x.MemberId,
                    MemberName = $"Member {x.MemberId}", // En producción, consultar Members para obtener nombre
                    ActionCount = x.Count,
                    LastActionAt = x.LastDate
                })
                .ToList();

            var suspiciousActions = audits
                .Where(a => a.Action == AuditActionType.EvidenceRejected || 
                           a.Action == AuditActionType.UnauthorizedAccess)
                .OrderByDescending(a => a.CreatedAt)
                .Take(20)
                .Select(a => MapToDto(a))
                .ToList();

            _logger.LogInformation(
                "Audit summary generated for tenant {TenantId}: {TotalRecords} records in {Days} days",
                tenantId, audits.Count, days);

            return new AuditSummaryDto
            {
                TenantId = tenantId,
                TotalRecords = audits.Count,
                DaysCovered = days,
                ActionCounts = actionCounts,
                EntityTypeCounts = entityTypeCounts,
                TopActiveMembers = topActiveMembers,
                SuspiciousActions = suspiciousActions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating audit summary for tenant {TenantId}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Mapea una entidad AuditLog a un DTO para transferencia de datos.
    /// </summary>
    private static AuditLogDto MapToDto(AuditLog auditLog) => new()
    {
        Id = auditLog.Id,
        ActorExternalSubjectId = auditLog.ActorExternalSubjectId,
        ActorMemberId = auditLog.ActorMemberId,
        Action = auditLog.Action,
        EntityType = auditLog.EntityType,
        EntityId = auditLog.EntityId,
        BeforeJson = auditLog.BeforeJson,
        AfterJson = auditLog.AfterJson,
        Notes = auditLog.Notes,
        CorrelationId = auditLog.CorrelationId,
        IpAddress = auditLog.IpAddress,
        UserAgent = auditLog.UserAgent,
        CreatedAt = auditLog.CreatedAt
    };
}
