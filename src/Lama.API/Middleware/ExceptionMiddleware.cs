namespace Lama.API.Middleware;

/// <summary>
/// Middleware para capturar y manejar excepciones de forma global
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada en {Path}: {Message}", context.Request.Path, ex.Message);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                status = context.Response.StatusCode,
                message = "Ocurrió un error interno",
                error = ex.Message,
                details = ex.StackTrace
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
