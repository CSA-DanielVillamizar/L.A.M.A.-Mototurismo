using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Lama.API.Controllers;

/// <summary>
/// Punto central para manejar errores no controlados y emitir ProblemDetails (RFC 7807).
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unexpected error",
            detail: exception?.Message ?? "An unexpected error occurred.",
            instance: HttpContext.Request.Path
        );
    }
}
