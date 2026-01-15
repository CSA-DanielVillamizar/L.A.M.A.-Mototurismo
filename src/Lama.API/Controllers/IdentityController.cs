using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lama.Application.Services;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para gestión de identidades de usuario vinculadas con Entra ID
/// Endpoints: 
/// - POST /api/identity/link - Vincular identidad Entra con miembro LAMA (admin)
/// - GET /api/identity/me - Obtener perfil del usuario actual autenticado
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityUserService _identityUserService;
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(
        IIdentityUserService identityUserService,
        ILogger<IdentityController> logger)
    {
        _identityUserService = identityUserService;
        _logger = logger;
    }

    /// <summary>
    /// Vincula una identidad de Entra ID con un miembro de LAMA
    /// Solo administrativos pueden vincular identidades
    /// </summary>
    /// <param name="request">Datos para vincular: externalSubjectId (Entra "sub") y memberId</param>
    /// <returns>Identidad vinculada con información de miembro asociado</returns>
    /// <response code="200">Identidad vinculada exitosamente</response>
    /// <response code="400">Solicitud inválida (datos faltantes o formato incorrecto)</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">No autorizado (se requieren permisos de admin)</response>
    /// <response code="404">Miembro no encontrado</response>
    [HttpPost("link")]
    [Authorize]
    public async Task<ActionResult<IdentityLinkResponse>> LinkIdentityToMember(
        [FromBody] LinkIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validar solicitud
        if (request == null || string.IsNullOrWhiteSpace(request.ExternalSubjectId) || request.MemberId <= 0)
        {
            _logger.LogWarning("Solicitud inválida de vinculación: externalSubjectId={ExternalSubjectId}, memberId={MemberId}",
                request?.ExternalSubjectId, request?.MemberId);
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid request",
                detail: "externalSubjectId y memberId son requeridos");
        }

        try
        {
            // Vincular identidad con miembro
            var identityUser = await _identityUserService.LinkToMemberAsync(
                request.ExternalSubjectId,
                request.MemberId,
                cancellationToken);

            _logger.LogInformation(
                "Identidad vinculada: externalSubjectId={ExternalSubjectId}, memberId={MemberId}, email={Email}",
                request.ExternalSubjectId,
                request.MemberId,
                identityUser.Email);

            return Ok(new IdentityLinkResponse
            {
                IdentityUserId = identityUser.Id,
                Email = identityUser.Email,
                DisplayName = identityUser.DisplayName,
                MemberId = identityUser.MemberId,
                ExternalSubjectId = identityUser.ExternalSubjectId,
                LinkedAt = identityUser.UpdatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Miembro no encontrado: {MemberId}", request.MemberId);
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Member not found",
                detail: $"Miembro con ID {request.MemberId} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error vinculando identidad: {ExternalSubjectId}", request.ExternalSubjectId);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Identity link failure",
                detail: "Error interno al vincular identidad");
        }
    }

    /// <summary>
    /// Obtiene el perfil del usuario autenticado actual
    /// Incluye información de identidad y miembro asociado (si existe)
    /// </summary>
    /// <returns>Perfil del usuario actual con su información de identidad</returns>
    /// <response code="200">Perfil obtenido exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<IdentityMeResponse>> GetCurrentUserProfile(
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtener identidad del usuario actual desde claims
            var identityUser = await _identityUserService.GetCurrentUserAsync(
                User,
                cancellationToken);

            if (identityUser == null)
            {
                _logger.LogWarning("Usuario autenticado no tiene registro de IdentityUser: {Email}",
                    User.FindFirst("email")?.Value);
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Identity not found",
                    detail: "Usuario no tiene registro de identidad");
            }

            return Ok(new IdentityMeResponse
            {
                IdentityUserId = identityUser.Id,
                Email = identityUser.Email,
                DisplayName = identityUser.DisplayName,
                ExternalSubjectId = identityUser.ExternalSubjectId,
                MemberId = identityUser.MemberId,
                MemberName = identityUser.Member?.CompleteNames,
                IsActive = identityUser.IsActive,
                CreatedAt = identityUser.CreatedAt,
                LastLoginAt = identityUser.LastLoginAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo perfil del usuario actual");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Profile retrieval failure",
                detail: "Error interno al obtener perfil");
        }
    }
}

/// <summary>
/// Solicitud para vincular una identidad de Entra ID con un miembro de LAMA
/// </summary>
public class LinkIdentityRequest
{
    /// <summary>
    /// Identificador único de Entra ID (claim "sub" del JWT)
    /// </summary>
    public string ExternalSubjectId { get; set; } = string.Empty;

    /// <summary>
    /// ID del miembro LAMA a vincular
    /// </summary>
    public int MemberId { get; set; }
}

/// <summary>
/// Respuesta después de vincular identidad exitosamente
/// </summary>
public class IdentityLinkResponse
{
    /// <summary>Identificador local de la identidad de usuario</summary>
    public int IdentityUserId { get; set; }

    /// <summary>Email del usuario de Entra ID</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre mostrado del usuario (opcional)</summary>
    public string? DisplayName { get; set; }

    /// <summary>Identificador de Entra ID (claim "sub" del JWT)</summary>
    public string ExternalSubjectId { get; set; } = string.Empty;

    /// <summary>ID del miembro LAMA vinculado</summary>
    public int? MemberId { get; set; }

    /// <summary>Timestamp cuando se vinculó la identidad</summary>
    public DateTime LinkedAt { get; set; }
}

/// <summary>
/// Respuesta del perfil del usuario actual
/// </summary>
public class IdentityMeResponse
{
    /// <summary>Identificador local de la identidad de usuario</summary>
    public int IdentityUserId { get; set; }

    /// <summary>Email del usuario</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre mostrado</summary>
    public string? DisplayName { get; set; }

    /// <summary>Identificador de Entra ID (claim "sub" del JWT)</summary>
    public string ExternalSubjectId { get; set; } = string.Empty;

    /// <summary>ID del miembro LAMA asociado (si existe)</summary>
    public int? MemberId { get; set; }

    /// <summary>Nombre del miembro LAMA asociado (si existe)</summary>
    public string? MemberName { get; set; }

    /// <summary>Si la identidad está activa</summary>
    public bool IsActive { get; set; }

    /// <summary>Cuando se creó la identidad</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Último login registrado</summary>
    public DateTime? LastLoginAt { get; set; }
}
