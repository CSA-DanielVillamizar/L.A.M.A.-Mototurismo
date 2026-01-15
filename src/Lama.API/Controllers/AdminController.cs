using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lama.Application.Services;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para operaciones administrativas
/// Endpoints para confirmación de asistencias con subida de evidencia
/// </summary>
[ApiController]
[Route("api/admin")]
public class AdminController(IAttendanceConfirmationService attendanceConfirmationService) : ControllerBase
{
    private readonly IAttendanceConfirmationService _attendanceConfirmationService = attendanceConfirmationService;

    /// <summary>
    /// Sube evidencia fotográfica y confirma asistencia de un miembro a un evento
    /// En Development mode (DEBUG), permite POST sin autenticación para testing
    /// En Production, requiere rol MTO o Admin
    /// </summary>
    /// <param name="eventId">ID del evento</param>
    /// <param name="memberId">ID del miembro (from body)</param>
    /// <param name="vehicleId">ID del vehículo (from body)</param>
    /// <param name="evidenceType">Tipo de evidencia: START_YEAR o CUTOFF</param>
    /// <param name="pilotWithBikePhoto">Foto: Piloto con moto</param>
    /// <param name="odometerCloseupPhoto">Foto: Odómetro close-up</param>
    /// <param name="odometerReading">Lectura del odómetro</param>
    /// <param name="unit">Unidad (Miles o Kilometers)</param>
    /// <param name="readingDate">Fecha de lectura (opcional)</param>
    /// <param name="notes">Notas adicionales (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <response code="200">Asistencia confirmada exitosamente</response>
    /// <response code="400">Solicitud inválida o datos faltantes</response>
    /// <response code="404">Evento, miembro o vehículo no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
#if !DEBUG
    [Authorize(Roles = "MTO,Admin")] // Solo autenticación en Production
#endif
    [HttpPost("evidence/upload")]
    [ProducesResponseType(typeof(EvidenceUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadEvidenceAsync(
        [FromQuery] int eventId,
        [FromForm] int memberId,
        [FromForm] int vehicleId,
        [FromForm] string evidenceType,
        [FromForm] IFormFile pilotWithBikePhoto,
        [FromForm] IFormFile odometerCloseupPhoto,
        [FromForm] double odometerReading,
        [FromForm] string unit,
        [FromForm] DateOnly? readingDate = null,
        [FromForm] string? notes = null,
        CancellationToken cancellationToken = default)
    {
        // Validación básica
        if (eventId <= 0 || memberId <= 0 || vehicleId <= 0)
            return BadRequest("eventId, memberId y vehicleId son requeridos y deben ser > 0");

        if (pilotWithBikePhoto == null || pilotWithBikePhoto.Length == 0)
            return BadRequest("Foto del piloto con moto es requerida");

        if (odometerCloseupPhoto == null || odometerCloseupPhoto.Length == 0)
            return BadRequest("Foto del odómetro es requerida");

        if (string.IsNullOrEmpty(evidenceType) || (evidenceType != "START_YEAR" && evidenceType != "CUTOFF"))
            return BadRequest("evidenceType debe ser START_YEAR o CUTOFF");

        if (string.IsNullOrEmpty(unit) || (unit != "Miles" && unit != "Kilometers"))
            return BadRequest("unit debe ser Miles o Kilometers");

        if (odometerReading <= 0)
            return BadRequest("odometerReading debe ser mayor a 0");

        try
        {
            // Obtener ID del usuario actual (MTO/Admin que valida)
            // Por ahora usar un ID hardcoded de demo; en producción usar User.FindFirst("sub").Value
            int validatedByMemberId = 1; // Placeholder

            // Crear solicitud
            using var pilotStream = pilotWithBikePhoto.OpenReadStream();
            using var odometerStream = odometerCloseupPhoto.OpenReadStream();

            var request = new UploadEvidenceRequest
            {
                MemberId = memberId,
                VehicleId = vehicleId,
                EvidenceType = evidenceType,
                PilotWithBikePhotoStream = pilotStream,
                PilotWithBikePhotoFileName = pilotWithBikePhoto.FileName,
                OdometerCloseupPhotoStream = odometerStream,
                OdometerCloseupPhotoFileName = odometerCloseupPhoto.FileName,
                OdometerReading = odometerReading,
                Unit = unit,
                ReadingDate = readingDate,
                Notes = notes
            };

            // Procesar confirmación
            var result = await _attendanceConfirmationService.ConfirmAttendanceAsync(
                eventId,
                request,
                validatedByMemberId,
                cancellationToken);

            if (!result.Success)
                return StatusCode(500, new { error = result.Message });

            // Devolver respuesta
            var response = new EvidenceUploadResponse
            {
                Message = result.Message,
                PointsAwarded = result.PointsAwardedPerMember ?? 0,
                PointsPerEvent = result.PointsPerEvent ?? 0,
                PointsPerDistance = result.PointsPerDistance ?? 0,
                VisitorClass = result.VisitorClass ?? "LOCAL",
                MemberId = result.MemberId ?? 0,
                VehicleId = result.VehicleId ?? 0,
                AttendanceId = result.AttendanceId ?? 0,
                EvidenceType = evidenceType
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error procesando solicitud", details = ex.Message });
        }
    }
}

/// <summary>
/// Respuesta de carga de evidencia exitosa
/// </summary>
public class EvidenceUploadResponse
{
    /// <summary>Mensaje de resultado</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Puntos totales otorgados</summary>
    public int PointsAwarded { get; set; }

    /// <summary>Puntos por evento</summary>
    public int PointsPerEvent { get; set; }

    /// <summary>Puntos por distancia</summary>
    public int PointsPerDistance { get; set; }

    /// <summary>Clasificación de visitante</summary>
    public string VisitorClass { get; set; } = string.Empty;

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>ID de la asistencia</summary>
    public int AttendanceId { get; set; }

    /// <summary>Tipo de evidencia subida</summary>
    public string EvidenceType { get; set; } = string.Empty;
}
