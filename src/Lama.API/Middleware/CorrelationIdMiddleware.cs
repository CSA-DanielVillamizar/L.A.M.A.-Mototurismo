namespace Lama.API.Middleware;

/// <summary>
/// Middleware que asegura que cada solicitud HTTP tenga un ID de correlación único.
/// El CorrelationId se usa para rastrear una solicitud a través de todo el sistema distribuido,
/// incluyendo logs y registros de auditoría.
/// 
/// Si el cliente envía X-Correlation-Id en el header, se usa ese valor.
/// Si no, se genera automáticamente un nuevo GUID.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private const string CorrelationIdItemKey = "CorrelationId";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Procesa la solicitud HTTP, asegurando la existencia de un CorrelationId.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Intentar obtener CorrelationId del header de solicitud
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            // Si no existe, generar uno nuevo
            correlationId = Guid.NewGuid().ToString();
        }

        // Almacenar CorrelationId en HttpContext para acceso en controladores/servicios
        context.Items[CorrelationIdItemKey] = correlationId;

        // Agregar CorrelationId al header de respuesta para que el cliente pueda rastrearlo
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
            return Task.CompletedTask;
        });

        _logger.LogInformation(
            "Request started: {Method} {Path} - CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            _logger.LogInformation(
                "Request completed: {Method} {Path} - Status: {StatusCode} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId);
        }
    }
}

/// <summary>
/// Extensiones de registro para facilitar el uso del middleware de CorrelationId.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>
    /// Registra el middleware de CorrelationId en el pipeline de solicitudes HTTP.
    /// Debe ser uno de los primeros middlewares para capturar todas las solicitudes.
    /// </summary>
    /// <param name="app">IApplicationBuilder.</param>
    /// <returns>IApplicationBuilder para encadenamiento fluido.</returns>
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
