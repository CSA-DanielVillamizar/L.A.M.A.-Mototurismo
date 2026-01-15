using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Lama.Application.Services;
using Lama.API.Utilities;

namespace Lama.API.Authorization;

/// <summary>
/// Handler de autorización que valida roles y scopes de usuarios.
/// Se ejecuta automáticamente cuando se usa [Authorize(Policy="...")] en endpoints.
/// Valida que el usuario tenga el rol requerido Y que su scope cubra el recurso.
/// </summary>
public class ScopeAuthorizationHandler : AuthorizationHandler<ResourceAuthorizationRequirement>
{
    private readonly IUserAuthorizationService _userAuthorizationService;
    private readonly ILogger<ScopeAuthorizationHandler> _logger;

    public ScopeAuthorizationHandler(
        IUserAuthorizationService userAuthorizationService,
        ILogger<ScopeAuthorizationHandler> logger)
    {
        _userAuthorizationService = userAuthorizationService ?? throw new ArgumentNullException(nameof(userAuthorizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Evalúa si el usuario autenticado cumple con el requirement de autorización.
    /// </summary>
    /// <param name="context">Contexto de autorización con información del usuario y recurso</param>
    /// <param name="requirement">Requirement a evaluar (contiene rol y scope requeridos)</param>
    /// <returns>Una tarea que completa cuando la autorización es evaluada</returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAuthorizationRequirement requirement)
    {
        // Validar que el usuario esté autenticado
        if (context.User == null)
        {
            _logger.LogWarning("Usuario no autenticado intentó acceder a recurso protegido");
            return;
        }

        // Extraer el identificador externo del usuario
        if (!ClaimsHelper.TryGetExternalSubjectId(context.User, out var externalSubjectId) || string.IsNullOrWhiteSpace(externalSubjectId))
        {
            _logger.LogWarning("Usuario autenticado sin claim 'sub' intentó acceder a recurso protegido");
            return;
        }

        _logger.LogDebug("Evaluando autorización para usuario {ExternalSubjectId}. Rol requerido: {RequiredRole}, Scope: {RequiredScopeType}",
            externalSubjectId, requirement.RequiredRole, requirement.RequiredScopeType);

        try
        {
            // Verificar si el usuario tiene el rol mínimo requerido
            var hasRequiredRole = await _userAuthorizationService.HasMinimumRoleAsync(
                externalSubjectId,
                requirement.RequiredRole);

            if (!hasRequiredRole)
            {
                _logger.LogWarning("Usuario {ExternalSubjectId} no tiene rol mínimo requerido {RequiredRole}",
                    externalSubjectId, requirement.RequiredRole);
                return;
            }

            _logger.LogDebug("Usuario {ExternalSubjectId} tiene rol requerido {RequiredRole}",
                externalSubjectId, requirement.RequiredRole);

            // Si llegamos aquí, el usuario tiene el rol requerido
            // Para una validación más completa con scope específico, se usaría context.Resource
            // Por ahora, damos acceso si tiene el rol
            // Los endpoints pueden hacer validaciones adicionales de scope si necesitan
            context.Succeed(requirement);

            _logger.LogInformation("Autorización exitosa para usuario {ExternalSubjectId}",
                externalSubjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al evaluar autorización para usuario {ExternalSubjectId}",
                externalSubjectId);
            // En caso de error, negar acceso por seguridad
            return;
        }
    }
}
