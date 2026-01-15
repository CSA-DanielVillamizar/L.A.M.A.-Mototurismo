using Lama.Application.Services;

namespace Lama.API.Middleware;

/// <summary>
/// Middleware que sincroniza la identidad de usuario después de autenticación exitosa
/// Crea o actualiza el registro de IdentityUser en la base de datos
/// </summary>
public class IdentityUserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentityUserSyncMiddleware> _logger;

    public IdentityUserSyncMiddleware(RequestDelegate next, ILogger<IdentityUserSyncMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IIdentityUserService identityUserService)
    {
        // Solo sincronizar si el usuario está autenticado
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Asegurar que existe un registro de IdentityUser y actualizar LastLoginAt
                await identityUserService.EnsureIdentityUserAsync(context.User);

                _logger.LogDebug(
                    "IdentityUser sincronizado para: {Email}",
                    context.User.FindFirst("email")?.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sincronizando IdentityUser para: {Email}",
                    context.User.FindFirst("email")?.Value);
                // No bloqueamos la solicitud si hay error en sincronización
                // Solo logueamos para auditoría
            }
        }

        await _next(context);
    }
}
