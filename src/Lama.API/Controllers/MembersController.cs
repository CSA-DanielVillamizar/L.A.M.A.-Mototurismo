using Microsoft.AspNetCore.Mvc;
using Lama.Application.Repositories;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de miembros
/// Proporciona endpoints para búsqueda de miembros y consulta de vehículos
/// </summary>
[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<MembersController> _logger;

    public MembersController(
        IMemberRepository memberRepository,
        IVehicleRepository vehicleRepository,
        ILogger<MembersController> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Busca miembros por nombre o apellido (autocomplete)
    /// </summary>
    /// <param name="q">Término de búsqueda (mínimo 2 caracteres)</param>
    /// <returns>Lista de miembros que coinciden con el término de búsqueda</returns>
    /// <response code="200">Lista de miembros encontrados</response>
    /// <response code="400">Término de búsqueda inválido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<MemberSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        {
            return BadRequest(new { error = "El término de búsqueda debe tener al menos 2 caracteres" });
        }

        try
        {
            var members = await _memberRepository.GetAllAsync();
            var searchTerm = q.ToLowerInvariant();

            var results = members
                .Where(m => 
                    m.CompleteNames.ToLowerInvariant().Contains(searchTerm))
                .Select(m => new MemberSearchDto
                {
                    MemberId = m.Id,
                    FirstName = m.CompleteNames.Split(' ').First(),
                    LastName = m.CompleteNames.Contains(' ') ? m.CompleteNames.Substring(m.CompleteNames.IndexOf(' ') + 1) : "",
                    FullName = m.CompleteNames,
                    Status = m.Status,
                    ChapterId = m.ChapterId
                })
                .OrderBy(m => m.FullName)
                .Take(20) // Limitar resultados para autocomplete
                .ToList();

            _logger.LogInformation("Búsqueda de miembros con término '{SearchTerm}': {Count} resultados", q, results.Count);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar miembros con término: {SearchTerm}", q);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al buscar miembros" });
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
                return NotFound(new { error = $"Miembro con ID {memberId} no encontrado" });
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
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener vehículos" });
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
                return NotFound(new { error = $"Miembro con ID {id} no encontrado" });
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
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener miembro" });
        }
    }
}
