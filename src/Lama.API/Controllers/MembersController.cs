using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Lama.Application.Repositories;
using Lama.Application.Services;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de miembros
/// Proporciona endpoints para búsqueda de miembros y consulta de vehículos
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(
        IMemberRepository memberRepository,
        IVehicleRepository vehicleRepository,
        ICacheService cacheService,
        ILogger<MembersController> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Busca miembros por nombre o apellido (autocomplete) - OPTIMIZADO para 4,000+ miembros
    /// </summary>
    /// <param name="q">Término de búsqueda (mínimo 2 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelación (AbortController desde frontend)</param>
    /// <returns>Lista de miembros que coinciden con el término de búsqueda</returns>
    /// <response code="200">Lista de miembros encontrados</response>
    /// <response code="400">Término de búsqueda inválido</response>
    /// <response code="429">Límite de tasa excedido (30 req/min)</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("search")]
    [EnableRateLimiting("search")]
    [ProducesResponseType(typeof(IEnumerable<MemberSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers(
        [FromQuery] string q, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid search term",
                detail: "El término de búsqueda debe tener al menos 2 caracteres"
            );
        }

        try
        {
            // Clave de caché: members:search:{term_normalizado}
            var cacheKey = $"members:search:{q.ToUpperInvariant().Trim()}";

            // Obtener del caché o ejecutar query (TTL 120s)
            var results = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    // OPTIMIZADO: Query directo a SQL con índice en CompleteNamesNormalized
                    var members = await _memberRepository.SearchByNameAsync(q, take: 20, cancellationToken);

                    return members.Select(m => new MemberSearchDto
                    {
                        MemberId = m.Id,
                        FirstName = m.CompleteNames.Split(' ').First(),
                        LastName = m.CompleteNames.Contains(' ') 
                            ? m.CompleteNames.Substring(m.CompleteNames.IndexOf(' ') + 1) 
                            : "",
                        FullName = m.CompleteNames,
                        Status = m.Status,
                        ChapterId = m.ChapterId
                    }).ToList();
                },
                TimeSpan.FromSeconds(120), // TTL 120 segundos
                cancellationToken);

            _logger.LogInformation("Búsqueda de miembros '{SearchTerm}': {Count} resultados", q, results.Count());
            return Ok(results);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Búsqueda cancelada por cliente: {SearchTerm}", q);
            return StatusCode(499, Problem(
                statusCode: 499,
                title: "Client Closed Request",
                detail: "Request cancelled by client",
                instance: HttpContext.Request.Path).Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar miembros con término: {SearchTerm}", q);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Search failure",
                detail: "Error al buscar miembros"
            );
        }
    }

    /// <summary>
    /// Obtiene todos los vehículos de un miembro específico
    /// </summary>
    /// <param name="memberId">ID del miembro</param>
    /// <returns>Lista de vehículos del miembro</returns>
    /// <response code="200">Lista de vehículos</response>
    /// <response code="404">Miembro no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{memberId}/vehicles")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMemberVehicles(int memberId)
    {
        try
        {
            // Verificar que el miembro existe
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                _logger.LogWarning("Miembro no encontrado: ID {MemberId}", memberId);
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Member not found",
                    detail: $"Miembro con ID {memberId} no encontrado"
                );
            }

            // Obtener vehículos del miembro
            var vehicles = await _vehicleRepository.GetByMemberAsync(memberId);
            var memberVehicles = vehicles
                .Select(v => new VehicleDto
                {
                    VehicleId = v.Id,
                    MemberId = v.MemberId,
                    LicPlate = v.LicPlate,
                    MotorcycleData = v.MotorcycleData,
                    Trike = v.Trike
                })
                .ToList();

            _logger.LogInformation("Se obtuvieron {Count} vehículos para miembro ID {MemberId}", memberVehicles.Count, memberId);
            return Ok(memberVehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos del miembro ID {MemberId}", memberId);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Vehicles retrieval failure",
                detail: "Error al obtener vehículos"
            );
        }
    }

    /// <summary>
    /// Obtiene un miembro específico por ID
    /// </summary>
    /// <param name="id">ID del miembro</param>
    /// <returns>Detalles del miembro</returns>
    /// <response code="200">Miembro encontrado</response>
    /// <response code="404">Miembro no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MemberDto>> GetMemberById(int id)
    {
        try
        {
            var member = await _memberRepository.GetByIdAsync(id);
            
            if (member == null)
            {
                _logger.LogWarning("Miembro no encontrado: ID {MemberId}", id);
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Member not found",
                    detail: $"Miembro con ID {id} no encontrado"
                );
            }

            var memberDto = new MemberDto
            {
                MemberId = member.Id,
                FirstName = member.CompleteNames.Split(' ').First(),
                LastName = member.CompleteNames.Contains(' ') ? member.CompleteNames.Substring(member.CompleteNames.IndexOf(' ') + 1) : "",
                FullName = member.CompleteNames,
                Status = member.Status,
                ChapterId = member.ChapterId
            };

            return Ok(memberDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener miembro ID {MemberId}", id);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Member retrieval failure",
                detail: "Error al obtener miembro"
            );
        }
    }
}
