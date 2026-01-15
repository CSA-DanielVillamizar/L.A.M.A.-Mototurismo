using Microsoft.AspNetCore.Mvc;
using Lama.Application.Repositories;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de eventos
/// Proporciona endpoints para listar eventos disponibles en el sistema
/// </summary>
[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventRepository eventRepository,
        ILogger<EventsController> logger)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los eventos activos disponibles para selección en formularios
    /// </summary>
    /// <returns>Lista de eventos ordenados por fecha descendente</returns>
    /// <response code="200">Lista de eventos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents([FromQuery] int? year = null)
    {
        try
        {
            var events = await _eventRepository.GetAllAsync();
            
            // Filtrar por año si se proporciona
            if (year.HasValue)
            {
                events = events.Where(e => e.EventStartDate.Year == year.Value);
            }
            
            var eventDtos = events.Select(e => new EventDto
            {
                EventId = e.Id,
                EventName = e.NameOfTheEvent,
                EventDate = e.EventStartDate,
                ChapterId = e.ChapterId,
                EventType = e.Class.ToString() // Convertir Class a string
            }).OrderByDescending(e => e.EventDate).ToList();

            _logger.LogInformation("Se obtuvieron {Count} eventos{YearFilter}", eventDtos.Count, year.HasValue ? $" para el año {year}" : "");
            return Ok(eventDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener eventos");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener eventos" });
        }
    }

    /// <summary>
    /// Obtiene un evento específico por ID
    /// </summary>
    /// <param name="id">ID del evento</param>
    /// <returns>Detalles del evento</returns>
    /// <response code="200">Evento encontrado</response>
    /// <response code="404">Evento no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventDto>> GetEventById(int id)
    {
        try
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            
            if (eventEntity == null)
            {
                _logger.LogWarning("Evento no encontrado: ID {EventId}", id);
                return NotFound(new { error = $"Evento con ID {id} no encontrado" });
            }

            var eventDto = new EventDto
            {
                EventId = eventEntity.Id,
                EventName = eventEntity.NameOfTheEvent,
                EventDate = eventEntity.EventStartDate,
                ChapterId = eventEntity.ChapterId,
                EventType = eventEntity.Class.ToString()
            };

            return Ok(eventDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener evento ID {EventId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener evento" });
        }
    }
}
