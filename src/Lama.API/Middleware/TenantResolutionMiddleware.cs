namespace Lama.API.Middleware;

/// <summary>
/// Middleware que resuelve el Tenant actual desde:
/// 1. Header X-Tenant (GUID)
/// 2. Claim "tenant_id" en JWT (si está autenticado)
/// 3. Subdominio (si se implementa en futuro)
/// 
/// Si ninguno está disponible, usa LAMA_DEFAULT (00000000-0000-0000-0000-000000000001)
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        var tenantId = ResolveTenantId(context);
        tenantContext.CurrentTenantId = tenantId;

        _logger.LogInformation(
            "TenantId resuelto: {TenantId} (Default={IsDefault}) para {Path}",
            tenantId,
            tenantContext.IsDefaultTenant,
            context.Request.Path);

        await _next(context);
    }

    /// <summary>
    /// Resuelve el TenantId desde múltiples fuentes, en orden de prioridad
    /// </summary>
    private Guid ResolveTenantId(HttpContext context)
    {
        // 1. Header X-Tenant (prioridad alta para testing/admin)
        if (context.Request.Headers.TryGetValue("X-Tenant", out var headerTenant))
        {
            if (Guid.TryParse(headerTenant.ToString(), out var tenantGuid))
            {
                return tenantGuid;
            }
        }

        // 2. JWT Claim "tenant_id" (si está autenticado)
        if (context.User?.FindFirst("tenant_id") is { } tenantClaim)
        {
            if (Guid.TryParse(tenantClaim.Value, out var tenantGuid))
            {
                return tenantGuid;
            }
        }

        // 3. Subdominio (implementable en futuro para SaaS)
        // var host = context.Request.Host.Host;
        // Si el formato es "tenant-name.lama.com", extraer "tenant-name"
        // Buscar en BD y obtener GUID del tenant
        // (Por ahora no implementado)

        // Default: LAMA_DEFAULT
        return TenantContext.DefaultTenantId;
    }
}
