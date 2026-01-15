using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Lama.API.Models.Evidence;
using Lama.API.Utilities;
using Lama.Application.Abstractions;
using Lama.Application.Services;
using Lama.Domain.Entities;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de evidencias fotográficas de miembros
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EvidenceController : ControllerBase
{
    private readonly ILamaDbContext _dbContext;
    private readonly IBlobSasService _blobSasService;
    private readonly IPointsCalculatorService _pointsCalculatorService;
    private readonly IRankingService _rankingService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<EvidenceController> _logger;

    public EvidenceController(
        ILamaDbContext dbContext,
        IBlobSasService blobSasService,
        IPointsCalculatorService pointsCalculatorService,
        IRankingService rankingService,
        ITenantProvider tenantProvider,
        ILogger<EvidenceController> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _blobSasService = blobSasService ?? throw new ArgumentNullException(nameof(blobSasService));
        _pointsCalculatorService = pointsCalculatorService ?? throw new ArgumentNullException(nameof(pointsCalculatorService));
        _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private ActionResult ProblemResponse(int statusCode, string title, string detail)
    {
        return Problem(statusCode: statusCode, title: title, detail: detail);
    }

    /// <summary>
    /// Endpoint 1: Solicita SAS URLs para upload directo de evidencias a Azure Blob
    /// Rate limited: 10 requests/min por IP
    /// </summary>
    [HttpPost("upload-request")]
    [EnableRateLimiting("upload")]
    [ProducesResponseType(typeof(EvidenceUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EvidenceUploadResponseDto>> RequestUploadSasAsync(
        [FromBody] EvidenceUploadRequestDto request)
    {
        try
        {
            // Validar tipo de evidencia
            if (request.EvidenceType != "START_YEAR" && request.EvidenceType != "CUTOFF")
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "Invalid evidence type", "EvidenceType debe ser 'START_YEAR' o 'CUTOFF'");
            }

            // Validar que si es CUTOFF, debe tener EventId
            if (request.EvidenceType == "CUTOFF" && !request.EventId.HasValue)
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "EventId requerido", "CUTOFF requiere EventId");
            }

            // Validar que el miembro existe
            var member = await _dbContext.Members.FindAsync(request.MemberId);
            if (member == null)
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "Miembro no encontrado", $"Miembro con ID {request.MemberId} no existe");
            }

            // Validar que el vehículo existe y pertenece al miembro
            var vehicle = await _dbContext.Vehicles
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.MemberId == request.MemberId);
            
            if (vehicle == null)
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "Vehículo inválido", $"Vehículo con ID {request.VehicleId} no existe o no pertenece al miembro");
            }

            // Validar que el evento existe (si es CUTOFF)
            if (request.EventId.HasValue)
            {
                var eventEntity = await _dbContext.Events.FindAsync(request.EventId.Value);
                if (eventEntity == null)
                {
                    return ProblemResponse(StatusCodes.Status400BadRequest, "Evento no encontrado", $"Evento con ID {request.EventId} no existe");
                }
            }

            // Generar correlationId único
            var correlationId = Guid.NewGuid().ToString("N");

            _logger.LogInformation("Generando SAS URLs para evidencia. CorrelationId: {CorrelationId}, MemberId: {MemberId}, EvidenceType: {EvidenceType}",
                correlationId, request.MemberId, request.EvidenceType);

            // Generar SAS URLs
            var sasResult = await _blobSasService.GenerateEvidenceUploadSasAsync(
                _tenantProvider.CurrentTenantId,
                request.EventId,
                request.MemberId,
                request.VehicleId,
                request.EvidenceType,
                correlationId,
                request.PilotPhotoContentType,
                request.OdometerPhotoContentType);

            var response = new EvidenceUploadResponseDto
            {
                CorrelationId = correlationId,
                PilotPhotoSasUrl = sasResult.PilotPhotoSasUrl,
                OdometerPhotoSasUrl = sasResult.OdometerPhotoSasUrl,
                PilotPhotoBlobPath = sasResult.PilotPhotoBlobPath,
                OdometerPhotoBlobPath = sasResult.OdometerPhotoBlobPath,
                ExpiresAt = sasResult.ExpiresAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar SAS URLs para evidencia");
            return ProblemResponse(StatusCodes.Status500InternalServerError, "Server error", "Error interno del servidor");
        }
    }

    /// <summary>
    /// Endpoint 2: Envía metadata de evidencia después de subir fotos a Blob
    /// Crea el registro de Evidence y opcionalmente Attendance (si es CUTOFF)
    /// </summary>
    [HttpPost("submit")]
    [ProducesResponseType(typeof(EvidenceSubmitResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EvidenceSubmitResponseDto>> SubmitEvidenceAsync(
        [FromBody] EvidenceSubmitRequestDto request)
    {
        try
        {
            // Validar tipo de evidencia
            if (!Enum.TryParse<EvidenceType>(request.EvidenceType, true, out var evidenceType))
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "Invalid evidence type", "EvidenceType inválido");
            }

            // Validar que si es CUTOFF, debe tener EventId
            if (evidenceType == EvidenceType.CUTOFF && !request.EventId.HasValue)
            {
                return ProblemResponse(StatusCodes.Status400BadRequest, "EventId requerido", "CUTOFF requiere EventId");
            }

            // Verificar que los blobs existen en Azure Storage
            var pilotExists = await _blobSasService.BlobExistsAsync(request.PilotPhotoBlobPath);
            var odometerExists = await _blobSasService.BlobExistsAsync(request.OdometerPhotoBlobPath);

            if (!pilotExists || !odometerExists)
            {
                _logger.LogWarning("Blobs no encontrados. CorrelationId: {CorrelationId}, PilotExists: {PilotExists}, OdometerExists: {OdometerExists}",
                    request.CorrelationId, pilotExists, odometerExists);
                
                return ProblemResponse(StatusCodes.Status400BadRequest, "Blobs faltantes", "Las fotos no se encontraron en el servidor. Por favor, vuelve a subirlas.");
            }

            // Crear registro de Evidence
            var evidence = new Evidence
            {
                TenantId = _tenantProvider.CurrentTenantId,
                CorrelationId = request.CorrelationId,
                MemberId = request.MemberId,
                VehicleId = request.VehicleId,
                EventId = request.EventId,
                EvidenceType = evidenceType,
                Status = EvidenceStatus.PENDING_REVIEW,
                PilotPhotoBlobPath = request.PilotPhotoBlobPath,
                OdometerPhotoBlobPath = request.OdometerPhotoBlobPath,
                OdometerReading = request.OdometerReading,
                OdometerUnit = request.OdometerUnit,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Evidences.Add(evidence);

            // Si es CUTOFF, crear Attendance en estado PENDING
            int? attendanceId = null;
            if (evidenceType == EvidenceType.CUTOFF && request.EventId.HasValue)
            {
                var attendance = new Attendance
                {
                    TenantId = _tenantProvider.CurrentTenantId,
                    EventId = request.EventId.Value,
                    MemberId = request.MemberId,
                    VehicleId = request.VehicleId,
                    Status = "PENDING",
                    ConfirmedAt = null,
                    // Puntos se calcularán en la aprobación
                    PointsPerEvent = null,
                    PointsPerDistance = null,
                    PointsAwardedPerMember = null
                };

                _dbContext.Attendance.Add(attendance);
                await _dbContext.SaveChangesAsync();

                evidence.AttendanceId = attendance.Id;
                attendanceId = attendance.Id;
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Evidencia creada exitosamente. EvidenceId: {EvidenceId}, CorrelationId: {CorrelationId}, AttendanceId: {AttendanceId}",
                evidence.Id, request.CorrelationId, attendanceId);

            var response = new EvidenceSubmitResponseDto
            {
                EvidenceId = evidence.Id,
                CorrelationId = request.CorrelationId,
                Status = evidence.Status.ToString(),
                AttendanceId = attendanceId,
                Message = evidenceType == EvidenceType.CUTOFF
                    ? "Evidencia enviada correctamente. Está pendiente de revisión por el MTO."
                    : "Evidencia de inicio de año enviada correctamente."
            };

            return CreatedAtAction(nameof(GetEvidenceByIdAsync), new { id = evidence.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar evidencia. CorrelationId: {CorrelationId}", request.CorrelationId);
            return ProblemResponse(StatusCodes.Status500InternalServerError, "Server error", "Error interno del servidor");
        }
    }

    /// <summary>
    /// Endpoint 3: Revisar evidencia (aprobar/rechazar) - Solo Admin/MTO
    /// </summary>
    [HttpPost("review")]
    [Authorize(Policy = "CanValidateEvent")]
    [ProducesResponseType(typeof(EvidenceReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvidenceReviewResponseDto>> ReviewEvidenceAsync(
        [FromBody] EvidenceReviewRequestDto request)
    {
        try
        {
            // Validar acción
            if (request.Action != "approve" && request.Action != "reject")
            {
                return BadRequest(new { error = "Action debe ser 'approve' o 'reject'" });
            }

            // Validar que si es reject, debe tener notas
            if (request.Action == "reject" && string.IsNullOrWhiteSpace(request.ReviewNotes))
            {
                return BadRequest(new { error = "ReviewNotes es obligatorio para rechazar" });
            }

            // Obtener evidencia con navegaciones
            var evidence = await _dbContext.Evidences
                .Include(e => e.Member)
                .Include(e => e.Vehicle)
                .Include(e => e.Event)
                .Include(e => e.Attendance)
                .FirstOrDefaultAsync(e => e.Id == request.EvidenceId);

            if (evidence == null)
            {
                return NotFound(new { error = $"Evidencia con ID {request.EvidenceId} no existe" });
            }

            // Validar que la evidencia está en estado PENDING_REVIEW
            if (evidence.Status != EvidenceStatus.PENDING_REVIEW)
            {
                return BadRequest(new { error = $"La evidencia ya fue revisada. Estado actual: {evidence.Status}" });
            }

            var userId = ClaimsHelper.GetExternalSubjectId(User);

            // Obtener el miembro asociado al usuario revisor
            var reviewerUser = await _dbContext.IdentityUsers.FirstOrDefaultAsync(u => u.ExternalSubjectId == userId);
            if (reviewerUser?.MemberId == null)
            {
                return BadRequest(new { Error = "Usuario revisor no asociado a un miembro" });
            }

            int reviewerId = reviewerUser.MemberId.Value;

            // Actualizar evidencia
            evidence.Status = request.Action == "approve" ? EvidenceStatus.APPROVED : EvidenceStatus.REJECTED;
            evidence.ReviewedAt = DateTime.UtcNow;
            evidence.ReviewedBy = reviewerId.ToString(); // Guardar como string para mantener compatibilidad
            evidence.ReviewNotes = request.ReviewNotes;
            evidence.UpdatedAt = DateTime.UtcNow;

            int? pointsAwarded = null;

            // Si se aprueba y es CUTOFF, procesar la asistencia
            if (request.Action == "approve" && evidence.EvidenceType == EvidenceType.CUTOFF && evidence.AttendanceId.HasValue)
            {
                var attendance = await _dbContext.Attendance
                    .Include(a => a.Event)
                    .Include(a => a.Member)
                    .Include(a => a.Vehicle)
                    .FirstOrDefaultAsync(a => a.Id == evidence.AttendanceId.Value);

                if (attendance != null && attendance.Event != null && attendance.Member != null && attendance.Vehicle != null)
                {
                    _logger.LogInformation("Procesando asistencia para evidencia aprobada. AttendanceId: {AttendanceId}", attendance.Id);

                    // 1. Actualizar odómetro del vehículo
                    attendance.Vehicle.FinalOdometer = (double)evidence.OdometerReading; // Cast decimal a double
                    attendance.Vehicle.FinalOdometerDate = DateOnly.FromDateTime(evidence.CreatedAt);
                    attendance.Vehicle.CutOffEvidenceUrl = evidence.PilotPhotoBlobPath; // Referencia al blob
                    attendance.Vehicle.CutOffEvidenceValidatedAt = DateTime.UtcNow;
                    attendance.Vehicle.OdometerUnit = evidence.OdometerUnit;
                    attendance.Vehicle.EvidenceValidatedBy = reviewerId; // reviewerId es int
                    attendance.Vehicle.Photography = "VALIDATED";
                    attendance.Vehicle.UpdatedAt = DateTime.UtcNow;

                    // 2. Calcular puntos usando IPointsCalculatorService
                    var pointsCalculation = await _pointsCalculatorService.CalculateAsync(
                        attendance.Event.Mileage,
                        attendance.Event.Class,
                        attendance.Member.CountryBirth,
                        attendance.Member.Continent,
                        attendance.Event.StartLocationCountry,
                        attendance.Event.StartLocationContinent);

                    // 3. Confirmar asistencia con puntos
                    attendance.Status = "CONFIRMED";
                    attendance.PointsPerEvent = pointsCalculation.PointsPerEvent;
                    attendance.PointsPerDistance = pointsCalculation.PointsPerDistance;
                    attendance.PointsAwardedPerMember = pointsCalculation.TotalPoints;
                    attendance.VisitorClass = pointsCalculation.VisitorClassification.ToString();
                    attendance.ConfirmedAt = DateTime.UtcNow;
                    attendance.ConfirmedBy = reviewerId;
                    attendance.UpdatedAt = DateTime.UtcNow;

                    pointsAwarded = pointsCalculation.TotalPoints;

                    _logger.LogInformation("Asistencia confirmada. AttendanceId: {AttendanceId}, Puntos: {Points}",
                        attendance.Id, pointsAwarded);

                    // 4. Actualizar ranking (MVP: actualización incremental inmediata)
                    var rankingEvent = new AttendanceConfirmedEvent
                    {
                        AttendanceId = attendance.Id,
                        MemberId = attendance.MemberId,
                        EventId = attendance.EventId,
                        Year = DateTime.UtcNow.Year,
                        PointsAwarded = pointsCalculation.TotalPoints,
                        MilesRecorded = attendance.Event.Mileage,
                        ScopeType = "GLOBAL", // Por defecto GLOBAL, puede extenderse a múltiples ámbitos
                        ScopeId = "GLOBAL",
                        VisitorClass = pointsCalculation.VisitorClassification.ToString(),
                        ConfirmedAt = DateTime.UtcNow
                    };

                    var rankingResult = await _rankingService.UpdateIncrementalAsync(
                        _tenantProvider.CurrentTenantId,
                        rankingEvent);

                    if (!rankingResult.Success)
                    {
                        _logger.LogWarning(
                            "Fallo al actualizar ranking incremental. AttendanceId: {AttendanceId}, Message: {Message}",
                            attendance.Id, rankingResult.Message);
                        // Continuar aunque falle el ranking (podría recuperarse en rebuild nocturno)
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Ranking actualizado. AttendanceId: {AttendanceId}, NewRank: {NewRank}",
                            attendance.Id, rankingResult.CurrentRank);
                    }
                }
            }

            // Si se rechaza y existe attendance, eliminarla
            if (request.Action == "reject" && evidence.AttendanceId.HasValue)
            {
                var attendance = await _dbContext.Attendance.FindAsync(evidence.AttendanceId.Value);
                if (attendance != null)
                {
                    _dbContext.Attendance.Remove(attendance);
                    evidence.AttendanceId = null;
                    _logger.LogInformation("Asistencia eliminada por rechazo de evidencia. AttendanceId: {AttendanceId}", attendance.Id);
                }
            }

            await _dbContext.SaveChangesAsync();

            var message = request.Action == "approve"
                ? "Evidencia aprobada exitosamente."
                : "Evidencia rechazada.";

            var response = new EvidenceReviewResponseDto
            {
                EvidenceId = evidence.Id,
                Status = evidence.Status.ToString(),
                Message = message,
                AttendanceId = evidence.AttendanceId,
                PointsAwarded = pointsAwarded
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar evidencia. EvidenceId: {EvidenceId}", request.EvidenceId);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener detalles de una evidencia por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EvidenceListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvidenceListItemDto>> GetEvidenceByIdAsync(int id)
    {
        try
        {
            var evidence = await _dbContext.Evidences
                .Include(e => e.Member)
                .Include(e => e.Vehicle)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evidence == null)
            {
                return NotFound(new { error = $"Evidencia con ID {id} no existe" });
            }

            // Generar SAS URLs de lectura para las fotos
            var pilotPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.PilotPhotoBlobPath, 60);
            var odometerPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.OdometerPhotoBlobPath, 60);

            var dto = new EvidenceListItemDto
            {
                Id = evidence.Id,
                CorrelationId = evidence.CorrelationId,
                MemberId = evidence.MemberId,
                MemberName = evidence.Member?.CompleteNames ?? "N/A",
                VehicleId = evidence.VehicleId,
                VehiclePlate = evidence.Vehicle?.LicPlate ?? "N/A",
                EventId = evidence.EventId,
                EventName = evidence.Event?.NameOfTheEvent ?? "N/A",
                EvidenceType = evidence.EvidenceType.ToString(),
                Status = evidence.Status.ToString(),
                OdometerReading = evidence.OdometerReading,
                OdometerUnit = evidence.OdometerUnit,
                CreatedAt = evidence.CreatedAt,
                ReviewedAt = evidence.ReviewedAt,
                ReviewedBy = evidence.ReviewedBy,
                ReviewNotes = evidence.ReviewNotes,
                PilotPhotoUrl = pilotPhotoUrl,
                OdometerPhotoUrl = odometerPhotoUrl
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener evidencia. EvidenceId: {EvidenceId}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Listar evidencias pendientes de revisión (para Admin/MTO)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Policy = "CanValidateEvent")]
    [ProducesResponseType(typeof(List<EvidenceListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EvidenceListItemDto>>> GetPendingEvidencesAsync()
    {
        try
        {
            var evidences = await _dbContext.Evidences
                .Include(e => e.Member)
                .Include(e => e.Vehicle)
                .Include(e => e.Event)
                .Where(e => e.Status == EvidenceStatus.PENDING_REVIEW)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            var dtos = new List<EvidenceListItemDto>();

            foreach (var evidence in evidences)
            {
                // Generar SAS URLs de lectura
                var pilotPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.PilotPhotoBlobPath, 60);
                var odometerPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.OdometerPhotoBlobPath, 60);

                dtos.Add(new EvidenceListItemDto
                {
                    Id = evidence.Id,
                    CorrelationId = evidence.CorrelationId,
                    MemberId = evidence.MemberId,
                    MemberName = evidence.Member?.CompleteNames ?? "N/A",
                    VehicleId = evidence.VehicleId,
                    VehiclePlate = evidence.Vehicle?.LicPlate ?? "N/A",
                    EventId = evidence.EventId,
                    EventName = evidence.Event?.NameOfTheEvent,
                    EvidenceType = evidence.EvidenceType.ToString(),
                    Status = evidence.Status.ToString(),
                    OdometerReading = evidence.OdometerReading,
                    OdometerUnit = evidence.OdometerUnit,
                    CreatedAt = evidence.CreatedAt,
                    ReviewedAt = evidence.ReviewedAt,
                    ReviewedBy = evidence.ReviewedBy,
                    ReviewNotes = evidence.ReviewNotes,
                    PilotPhotoUrl = pilotPhotoUrl,
                    OdometerPhotoUrl = odometerPhotoUrl
                });
            }

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar evidencias pendientes");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Listar mis evidencias (usuario actual)
    /// </summary>
    [HttpGet("my-evidences")]
    [ProducesResponseType(typeof(List<EvidenceListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EvidenceListItemDto>>> GetMyEvidencesAsync()
    {
        try
        {
            var userId = ClaimsHelper.GetExternalSubjectId(User);

            // Obtener el miembro asociado al usuario actual
            var identityUser = await _dbContext.IdentityUsers
                .FirstOrDefaultAsync(u => u.ExternalSubjectId == userId);

            if (identityUser?.MemberId == null)
            {
                return Ok(new List<EvidenceListItemDto>()); // Usuario no tiene miembro asociado
            }

            var evidences = await _dbContext.Evidences
                .Include(e => e.Member)
                .Include(e => e.Vehicle)
                .Include(e => e.Event)
                .Where(e => e.MemberId == identityUser.MemberId.Value)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            var dtos = new List<EvidenceListItemDto>();

            foreach (var evidence in evidences)
            {
                // Generar SAS URLs de lectura
                var pilotPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.PilotPhotoBlobPath, 60);
                var odometerPhotoUrl = await _blobSasService.GenerateReadSasUrlAsync(evidence.OdometerPhotoBlobPath, 60);

                dtos.Add(new EvidenceListItemDto
                {
                    Id = evidence.Id,
                    CorrelationId = evidence.CorrelationId,
                    MemberId = evidence.MemberId,
                    MemberName = evidence.Member?.CompleteNames ?? "N/A",
                    VehicleId = evidence.VehicleId,
                    VehiclePlate = evidence.Vehicle?.LicPlate ?? "N/A",
                    EventId = evidence.EventId,
                    EventName = evidence.Event?.NameOfTheEvent,
                    EvidenceType = evidence.EvidenceType.ToString(),
                    Status = evidence.Status.ToString(),
                    OdometerReading = evidence.OdometerReading,
                    OdometerUnit = evidence.OdometerUnit,
                    CreatedAt = evidence.CreatedAt,
                    ReviewedAt = evidence.ReviewedAt,
                    ReviewedBy = evidence.ReviewedBy,
                    ReviewNotes = evidence.ReviewNotes,
                    PilotPhotoUrl = pilotPhotoUrl,
                    OdometerPhotoUrl = odometerPhotoUrl
                });
            }

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar mis evidencias");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
