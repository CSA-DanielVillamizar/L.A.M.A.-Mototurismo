using Microsoft.AspNetCore.Mvc;
using Lama.Application.Services;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para tipos de estados de miembros
/// Proporciona endpoints para obtener valores de dropdowns en COR
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MemberStatusTypesController : ControllerBase
{
    private readonly IMemberStatusService _statusService;
    private readonly ILogger<MemberStatusTypesController> _logger;

    public MemberStatusTypesController(
        IMemberStatusService statusService,
        ILogger<MemberStatusTypesController> logger)
    {
        _statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los tipos de estado disponibles para dropdown
    /// </summary>
    /// <returns>Lista de tipos de estado ordenados por DisplayOrder</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberStatusTypeDto>>> GetAllStatusTypes()
    {
        try
        {
            var statusTypes = await _statusService.GetAllStatusTypesAsync();
            var dtos = statusTypes.ToDto().ToList();
            
            _logger.LogInformation("Se obtuvieron {Count} tipos de estado", dtos.Count);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de estado");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener tipos de estado");
        }
    }

    /// <summary>
    /// Obtiene tipos de estado filtrados por categoría
    /// Categorías disponibles: CHAPTER, CHAPTER_OFFICER, REGIONAL_OFFICER, NATIONAL_OFFICER, CONTINENTAL_OFFICER, INTERNATIONAL_OFFICER
    /// </summary>
    /// <param name="category">Nombre de la categoría</param>
    /// <returns>Lista de tipos de estado de la categoría especificada</returns>
    [HttpGet("by-category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MemberStatusTypeDto>>> GetStatusTypesByCategory(string category)
    {
        try
        {
            var statusTypes = await _statusService.GetStatusTypesByCategoryAsync(category);
            var dtos = statusTypes.ToDto().ToList();

            if (!dtos.Any())
            {
                _logger.LogWarning("No se encontraron tipos de estado para categoría: {Category}", category);
                return NotFound($"No hay tipos de estado para la categoría: {category}");
            }

            _logger.LogInformation("Se obtuvieron {Count} tipos de estado para categoría {Category}", dtos.Count, category);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de estado por categoría: {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener tipos de estado");
        }
    }

    /// <summary>
    /// Obtiene todas las categorías de estados disponibles
    /// Útil para agrupar opciones en el dropdown del COR
    /// </summary>
    /// <returns>Lista de nombres de categorías</returns>
    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        try
        {
            var categories = await _statusService.GetAllCategoriesAsync();
            var categoryList = categories.ToList();
            
            _logger.LogInformation("Se obtuvieron {Count} categorías de estado", categoryList.Count);
            return Ok(categoryList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías de estado");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener categorías de estado");
        }
    }

    /// <summary>
    /// Obtiene un tipo de estado específico por nombre
    /// </summary>
    /// <param name="statusName">Nombre del estado (ej: "CHAPTER PRESIDENT")</param>
    /// <returns>El tipo de estado solicitado</returns>
    [HttpGet("by-name/{statusName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberStatusTypeDto>> GetStatusTypeByName(string statusName)
    {
        try
        {
            var statusType = await _statusService.GetStatusTypeByNameAsync(statusName);
            
            if (statusType == null)
            {
                _logger.LogWarning("Tipo de estado no encontrado: {StatusName}", statusName);
                return NotFound($"Tipo de estado no encontrado: {statusName}");
            }

            return Ok(statusType.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de estado por nombre: {StatusName}", statusName);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener tipo de estado");
        }
    }
}
