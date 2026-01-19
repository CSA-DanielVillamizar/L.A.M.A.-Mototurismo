using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lama.Application.Repositories;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para operaciones administrativas de eventos
/// Endpoints exclusivos para usuarios con rol ADMIN o superior
/// </summary>
[ApiController]
[Route("api/v1/admin/events")]
[Authorize(Policy = "AdminOnly")]
public class AdminEventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<AdminEventsController> _logger;

    public AdminEventsController(
        IEventRepository eventRepository,
        IAttendanceRepository attendanceRepository,
        IMemberRepository memberRepository,
        IVehicleRepository vehicleRepository,
        ILogger<AdminEventsController> logger)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _attendanceRepository = attendanceRepository ?? throw new ArgumentNullException(nameof(attendanceRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene lista de asistentes a un evento con estado específico
    /// </summary>
    /// <param name="eventId">ID del evento</param>
    /// <param name="status">Estado de asistencia: PENDING o CONFIRMED (opcional, retorna todos si no se especifica)</param>
    /// <returns>Lista de asistentes con sus detalles</returns>
    /// <response code="200">Lista de asistentes</response>
    /// <response code="404">Evento no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{eventId}/attendees")]
    [ProducesResponseType(typeof(IEnumerable<AttendeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AttendeeDto>>> GetEventAttendees(
        int eventId,
        [FromQuery] string? status = null)
    {
        try
        {
            // Verificar que el evento existe
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Evento no encontrado: ID {EventId}", eventId);
                return NotFound(new { error = $"Evento con ID {eventId} no encontrado" });
            }

            var attendances = await _attendanceRepository.GetByEventAsync(eventId);

            // Filtrar por estado si se proporciona
            if (!string.IsNullOrWhiteSpace(status))
            {
                attendances = attendances.Where(a => a.Status == status);
            }

            var attendeeDtos = new List<AttendeeDto>();

            foreach (var attendance in attendances)
            {
                var member = await _memberRepository.GetByIdAsync(attendance.MemberId);
                var vehicle = await _vehicleRepository.GetByIdAsync(attendance.VehicleId);

                if (member != null && vehicle != null)
                {
                    attendeeDtos.Add(new AttendeeDto
                    {
                        AttendanceId = attendance.Id,
                        MemberId = member.Id,
                        CompleteNames = member.CompleteNames,
                        Order = member.Order,
                        VehicleId = vehicle.Id,
                        LicPlate = vehicle.LicPlate,
                        MotorcycleData = vehicle.MotorcycleData,
                        Status = attendance.Status,
                        ConfirmedAt = attendance.ConfirmedAt
                    });
                }
            }

            _logger.LogInformation("Se obtuvieron {Count} asistentes para evento ID {EventId}{StatusFilter}",
                attendeeDtos.Count, eventId, !string.IsNullOrWhiteSpace(status) ? $" (estado: {status})" : "");

            return Ok(attendeeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistentes para evento ID {EventId}", eventId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener asistentes" });
        }
    }
}
