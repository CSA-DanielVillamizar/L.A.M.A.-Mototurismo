using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lama.Application.Repositories;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para operaciones administrativas de miembros
/// Endpoints exclusivos para usuarios con rol ADMIN o superior
/// </summary>
[ApiController]
[Route("api/v1/admin/members")]
[Authorize(Policy = "AdminOnly")]
public class AdminMembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<AdminMembersController> _logger;

    public AdminMembersController(
        IMemberRepository memberRepository,
        IVehicleRepository vehicleRepository,
        ILogger<AdminMembersController> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Busca miembros por nombre, orden o placa de vehículo (para COR)
    /// </summary>
    /// <param name="q">Término de búsqueda: nombre completo, número de orden o placa</param>
    /// <returns>Lista de miembros que coinciden con el término</returns>
    /// <response code="200">Lista de miembros encontrados</response>
    /// <response code="400">Término de búsqueda inválido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<MemberSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MemberSearchDto>>> SearchMembers([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 1)
        {
            return BadRequest(new { error = "El término de búsqueda es requerido" });
        }

        try
        {
            var members = await _memberRepository.GetAllAsync();
            var searchTerm = q.ToLowerInvariant();
            var isNumber = int.TryParse(q, out int orderNumber);

            var results = members
                .Where(m => 
                    // Búsqueda por nombre
                    m.CompleteNames.ToLowerInvariant().Contains(searchTerm) ||
                    // Búsqueda por orden exacto
                    (isNumber && m.Order == orderNumber))
                .Select(m => new MemberSearchDto
                {
                    MemberId = m.Id,
                    FirstName = m.CompleteNames.Split(' ').First(),
                    LastName = m.CompleteNames.Contains(' ') ? m.CompleteNames.Substring(m.CompleteNames.IndexOf(' ') + 1) : "",
                    FullName = m.CompleteNames,
                    Status = m.Status,
                    ChapterId = m.ChapterId,
                    Order = m.Order
                })
                .OrderBy(m => m.Order)
                .ThenBy(m => m.FullName)
                .Take(50)
                .ToList();

            _logger.LogInformation("Búsqueda admin de miembros con término '{SearchTerm}': {Count} resultados", q, results.Count);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar miembros con término: {SearchTerm}", q);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al buscar miembros" });
        }
    }

    /// <summary>
    /// Obtiene los vehículos de un miembro específico
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
            var member = await _memberRepository.GetByIdAsync(memberId);
            
            if (member == null)
            {
                _logger.LogWarning("Miembro no encontrado: ID {MemberId}", memberId);
                return NotFound(new { error = $"Miembro con ID {memberId} no encontrado" });
            }

            var vehicles = await _vehicleRepository.GetByMemberAsync(memberId);
            var vehicleDtos = vehicles.Select(v => new VehicleDto
            {
                VehicleId = v.Id,
                MemberId = v.MemberId,
                MotorcycleData = v.MotorcycleData,
                LicPlate = v.LicPlate,
                Trike = v.Trike
            }).ToList();

            _logger.LogInformation("Se obtuvieron {Count} vehículos para miembro ID {MemberId}", vehicleDtos.Count, memberId);
            return Ok(vehicleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos del miembro ID {MemberId}", memberId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener vehículos" });
        }
    }

    /// <summary>
    /// Obtiene detalles de un miembro específico
    /// </summary>
    /// <param name="memberId">ID del miembro</param>
    /// <returns>Detalles del miembro</returns>
    /// <response code="200">Detalles del miembro</response>
    /// <response code="404">Miembro no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{memberId}")]
    [ProducesResponseType(typeof(MemberSearchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MemberSearchDto>> GetMember(int memberId)
    {
        try
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            
            if (member == null)
            {
                _logger.LogWarning("Miembro no encontrado: ID {MemberId}", memberId);
                return NotFound(new { error = $"Miembro con ID {memberId} no encontrado" });
            }

            var memberDto = new MemberSearchDto
            {
                MemberId = member.Id,
                FirstName = member.CompleteNames.Split(' ').First(),
                LastName = member.CompleteNames.Contains(' ') ? member.CompleteNames.Substring(member.CompleteNames.IndexOf(' ') + 1) : "",
                FullName = member.CompleteNames,
                Status = member.Status,
                ChapterId = member.ChapterId,
                Order = member.Order
            };

            return Ok(memberDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener miembro ID {MemberId}", memberId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error al obtener miembro" });
        }
    }
}
