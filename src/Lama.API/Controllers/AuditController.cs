using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lama.Application.Services;
using Lama.Domain.Enums;

namespace Lama.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(IAuditService auditService, ILogger<AuditController> logger)
    {
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene el historial de auditoría de un miembro específico.
    /// Útil para rastrear todas las acciones realizadas por un usuario.
    /// 
    /// Ejemplo: GET /api/audit/member/123?take=50
    /// </summary>
    /// <param name="memberId">ID del miembro cuyas acciones se quieren auditar.</param>
    /// <param name="take">Número máximo de registros a retornar (default 100, max 1000).</param>
    /// <returns>Lista de registros de auditoría del miembro.</returns>
    [HttpGet("member/{memberId:int}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditsByMember(
        [FromRoute] int memberId,
        [FromQuery] int take = 100)
    {
        try
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

            if (memberId <= 0)
                return BadRequest("Member ID must be greater than 0.");

            var audits = await _auditService.GetAuditsByMemberAsync(Guid.NewGuid(), memberId, take);

            _logger.LogInformation(
                "Retrieved audit logs for member {MemberId}. CorrelationId: {CorrelationId}",
                memberId, correlationId);

            return Ok(audits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for member {MemberId}", memberId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving audit logs.");
        }
    }

    /// <summary>
    /// Obtiene el historial de cambios de una entidad específica (Evidence, Event, Vehicle, etc.).
    /// Permite auditar todos los cambios realizados a una entidad en particular.
    /// 
    /// Ejemplo: GET /api/audit/entity/Evidence/123
    /// </summary>
    /// <param name="entityType">Tipo de entidad (Evidence, Attendance, Vehicle, Member, Event, MemberRole, MemberScope, Configuration, Chapter, Country, Continent, RankingSnapshot).</param>
    /// <param name="entityId">ID específico de la entidad.</param>
    /// <param name="take">Número máximo de registros a retornar (default 100, max 1000).</param>
    /// <returns>Lista de cambios registrados para la entidad.</returns>
    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditsByEntity(
        [FromRoute] string entityType,
        [FromRoute] string entityId,
        [FromQuery] int take = 100)
    {
        try
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

            // Validar que entityType sea un enum válido
            if (!Enum.TryParse<AuditEntityType>(entityType, ignoreCase: true, out var parsedEntityType))
            {
                var validTypes = string.Join(", ", Enum.GetNames(typeof(AuditEntityType)));
                return BadRequest($"Invalid entity type. Valid types: {validTypes}");
            }

            if (string.IsNullOrWhiteSpace(entityId))
                return BadRequest("Entity ID cannot be empty.");

            var audits = await _auditService.GetAuditsByEntityAsync(Guid.NewGuid(), parsedEntityType, entityId, take);

            _logger.LogInformation(
                "Retrieved audit logs for {EntityType} {EntityId}. CorrelationId: {CorrelationId}",
                entityType, entityId, correlationId);

            return Ok(audits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for {EntityType} {EntityId}", entityType, entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving audit logs.");
        }
    }

    /// <summary>
    /// Obtiene todos los logs asociados a un ID de correlación específico.
    /// Útil para rastrear una solicitud distribuida a través de múltiples servicios.
    /// 
    /// Ejemplo: GET /api/audit/correlation/550e8400-e29b-41d4-a716-446655440000
    /// </summary>
    /// <param name="correlationId">ID de correlación a rastrear.</param>
    /// <returns>Lista de registros de auditoría con el CorrelationId especificado.</returns>
    [HttpGet("correlation/{correlationId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditsByCorrelationId(
        [FromRoute] string correlationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(correlationId))
                return BadRequest("Correlation ID cannot be empty.");

            var audits = await _auditService.GetAuditsByCorrelationIdAsync(correlationId);

            _logger.LogInformation(
                "Retrieved audit logs for CorrelationId {CorrelationId}",
                correlationId);

            return Ok(audits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for CorrelationId {CorrelationId}", correlationId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving audit logs.");
        }
    }

    /// <summary>
    /// Obtiene un resumen agregado de auditoría para los últimos N días.
    /// Útil para análisis de seguridad, cumplimiento normativo, y detección de anomalías.
    /// 
    /// Ejemplo: GET /api/audit/summary?days=30
    /// 
    /// Retorna:
    /// - Conteo total de acciones
    /// - Desglose por tipo de acción (Create, Update, Delete, Approve, Reject, etc.)
    /// - Desglose por tipo de entidad (Evidence, Attendance, Vehicle, etc.)
    /// - Top 10 miembros más activos
    /// - Acciones sospechosas (rechazos, intentos de acceso no autorizado)
    /// </summary>
    /// <param name="days">Número de días hacia atrás a incluir en el resumen (default 30, max 365).</param>
    /// <returns>Resumen de auditoría del período especificado.</returns>
    [HttpGet("summary")]
    [Authorize(Policy = "IsSuperAdmin")]
    public async Task<ActionResult<AuditSummaryDto>> GetAuditSummary(
        [FromQuery] int days = 30)
    {
        try
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

            if (days <= 0 || days > 365)
                return BadRequest("Days must be between 1 and 365.");

            var summary = await _auditService.GetAuditSummaryAsync(Guid.NewGuid(), days);

            _logger.LogInformation(
                "Generated audit summary for {Days} days. CorrelationId: {CorrelationId}",
                days, correlationId);

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating audit summary");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the audit summary.");
        }
    }
}
