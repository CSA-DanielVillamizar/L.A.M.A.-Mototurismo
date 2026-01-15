using System.Text.Json;
using Lama.Application.Abstractions;
using Lama.Domain.Entities;
using Lama.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Lama.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor de EF Core que captura automáticamente cambios en entidades
/// y los registra en el audit log.
/// 
/// Se ejecuta ANTES de que SaveChanges persista los cambios en BD.
/// Serializa estados antes/después en JSON para auditoría completa.
/// </summary>
public class AuditLoggingInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditLoggingInterceptor> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLoggingInterceptor(
        ILogger<AuditLoggingInterceptor> logger,
        ITenantProvider tenantProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Se ejecuta ANTES de SaveChanges. Captura estados antes de cambios.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
            return result;

        CaptureAuditTrail(dbContext);
        return result;
    }

    /// <summary>
    /// Se ejecuta ANTES de SaveChangesAsync. Versión asincrónica.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
            return new ValueTask<InterceptionResult<int>>(result);

        CaptureAuditTrail(dbContext);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    /// <summary>
    /// Captura todos los cambios pendientes en la sesión EF Core y los convierte en registros de auditoría.
    /// </summary>
    private void CaptureAuditTrail(DbContext dbContext)
    {
        try
        {
            var tenantId = _tenantProvider.CurrentTenantId;
            
            // Obtener el subject externo del JWT (claim "sub")
            var actorExternalSubjectId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value 
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value 
                ?? "Unknown";
            
            // Obtener el memberId desde un claim personalizado (si está disponible) o del contexto
            // Por ahora, solo capturamos el que sea resolvible desde ApplicationDbContext
            int? actorMemberId = null;
            
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

            var changedEntries = dbContext.ChangeTracker
                .Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in changedEntries)
            {
                // Saltar entidades de auditoría mismas para evitar loops
                if (entry.Entity is AuditLog)
                    continue;

                var auditEntry = CreateAuditEntry(
                    entry,
                    tenantId,
                    actorExternalSubjectId,
                    actorMemberId,
                    correlationId,
                    ipAddress,
                    userAgent);

                if (auditEntry != null)
                {
                    dbContext.Set<AuditLog>().Add(auditEntry);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during audit trail capture in SaveChangesInterceptor");
            // No relanzamos excepción para no afectar la operación principal
        }
    }

    /// <summary>
    /// Crea un registro AuditLog basado en los cambios detectados en una entidad.
    /// </summary>
    private AuditLog? CreateAuditEntry(
        EntityEntry entry,
        Guid tenantId,
        string actorExternalSubjectId,
        int? actorMemberId,
        string? correlationId,
        string? ipAddress,
        string? userAgent)
    {
        // Mapear tipo de entidad a AuditEntityType
        AuditEntityType? entityType = GetAuditEntityType(entry.Entity.GetType());
        if (entityType == null)
            return null;

        // Obtener ID de la entidad (usa Id property principal)
        var entityId = GetEntityId(entry);
        if (string.IsNullOrEmpty(entityId))
            return null;

        // Mapear estado de EF a AuditActionType
        AuditActionType? action = entry.State switch
        {
            EntityState.Added => AuditActionType.Create,
            EntityState.Modified => AuditActionType.Update,
            EntityState.Deleted => AuditActionType.Delete,
            _ => null
        };

        if (action == null)
            return null;

        // Serializar estados JSON
        var beforeJson = entry.State == EntityState.Added ? null : SerializeProperties(entry, EntityState.Unchanged);
        var afterJson = entry.State == EntityState.Deleted ? null : SerializeProperties(entry, EntityState.Modified);

        return new AuditLog
        {
            TenantId = tenantId,
            ActorExternalSubjectId = actorExternalSubjectId,
            ActorMemberId = actorMemberId,
            Action = action.Value,
            EntityType = entityType.Value,
            EntityId = entityId,
            BeforeJson = beforeJson,
            AfterJson = afterJson,
            CorrelationId = correlationId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Obtiene el ID de una entidad desde su propiedad Id principal.
    /// </summary>
    private static string GetEntityId(EntityEntry entry)
    {
        var keyProperty = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (keyProperty == null)
            return string.Empty;

        var keyValue = entry.CurrentValues[keyProperty];
        return keyValue?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Serializa propiedades de una entidad a JSON.
    /// Excluye propiedades de navegación y shadow properties.
    /// </summary>
    private static string? SerializeProperties(EntityEntry entry, EntityState state)
    {
        var propertyValues = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            // Obtener el valor
            var value = state == EntityState.Modified
                ? property.CurrentValue
                : property.OriginalValue;

            propertyValues[property.Metadata.Name] = value;
        }

        if (propertyValues.Count == 0)
            return null;

        try
        {
            return JsonSerializer.Serialize(propertyValues, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea un tipo CLR a AuditEntityType.
    /// Extensible para nuevas entidades auditables.
    /// </summary>
    private static AuditEntityType? GetAuditEntityType(Type entityType)
    {
        var typeName = entityType.Name;
        
        return typeName switch
        {
            "Evidence" => AuditEntityType.Evidence,
            "Attendance" => AuditEntityType.Attendance,
            "Vehicle" => AuditEntityType.Vehicle,
            "Member" => AuditEntityType.Member,
            "Event" => AuditEntityType.Event,
            "UserRole" => AuditEntityType.MemberRole,
            "UserScope" => AuditEntityType.MemberScope,
            "RankingSnapshot" => AuditEntityType.RankingSnapshot,
            _ => null
        };
    }
}
