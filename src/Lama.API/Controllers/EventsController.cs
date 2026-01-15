using Microsoft.AspNetCore.Mvc;
using Lama.Application.Repositories;
using Lama.Application.Services;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de eventos
/// Proporciona endpoints para listar eventos disponibles en el sistema
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventRepository eventRepository,
        ICacheService cacheService,
        ILogger<EventsController> logger)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los eventos activos disponibles para selección en formularios - CACHEADO
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
            // Clave de caché: events:year:{year} o events:all
            var cacheKey = year.HasValue ? $"events:year:{year.Value}" : "events:all";

            // Obtener del caché o ejecutar query (TTL 300s = 5 minutos)
            var eventDtos = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var events = await _eventRepository.GetAllAsync();

                    // Filtrar por año si se proporciona
                    if (year.HasValue)
                    {
                        events = events.Where(e => e.EventStartDate.Year == year.Value);
                    }

                    return events.Select(e => new EventDto
                    {
                        EventId = e.Id,
                        EventName = e.NameOfTheEvent,
                        EventDate = e.EventStartDate,
                        ChapterId = e.ChapterId,
                        EventType = e.Class.ToString()
                    }).OrderByDescending(e => e.EventDate).ToList();
                },
                TimeSpan.FromSeconds(300)); // TTL 300 segundos (5 min)

            _logger.LogInformation("Se obtuvieron {Count} eventos{YearFilter}", eventDtos.Count(), 
                year.HasValue ? $" para el año {year}" : "");
            return Ok(eventDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener eventos");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Events retrieval failure",
                detail: "Error al obtener eventos");
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
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Event not found",
                    detail: $"Evento con ID {id} no encontrado");
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
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Event retrieval failure",
                detail: "Error al obtener evento");
        }
    }
}
