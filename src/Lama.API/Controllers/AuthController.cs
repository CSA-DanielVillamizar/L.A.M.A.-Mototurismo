using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Lama.API.Models.Auth;
using Lama.Application.Services;
using Lama.Application.DTOs;

namespace Lama.API.Controllers;

/// <summary>
/// Controlador para autenticación local (email/password y OAuth)
/// Endpoints:
/// - POST /api/auth/exchange - Intercambio de token de Entra ID (PRODUCTION)
/// - POST /api/auth/login - Login con email/password (DEV only)
/// - POST /api/auth/signup - Registración
/// - POST /api/auth/refresh - Renovar token
/// - POST /api/auth/logout - Logout (revocar refresh token)
/// - POST /api/auth/forgot-password - Solicitar reset de contraseña
/// - POST /api/auth/reset-password - Reset con token
/// - POST /api/auth/verify-email - Verificar email
/// - POST /api/auth/oauth/google - Login con Google
/// - POST /api/auth/oauth/github - Login con GitHub
/// - GET /api/auth/profile - Obtener perfil del usuario actual
/// - GET /api/auth/me - Obtener usuario autenticado (sesión app)
/// - POST /api/auth/change-password - Cambiar contraseña
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IIdentityUserService _identityUserService;
    private readonly IAuthSessionService _authSessionService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthenticationService authService,
        IIdentityUserService identityUserService,
        IAuthSessionService authSessionService,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _identityUserService = identityUserService ?? throw new ArgumentNullException(nameof(identityUserService));
        _authSessionService = authSessionService ?? throw new ArgumentNullException(nameof(authSessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Login con email y contraseña
    /// Rate limited: 10 intentos/minuto por IP
    /// </summary>
    /// <param name="request">Email y contraseña</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Access token + refresh token</returns>
    /// <response code="200">Login exitoso</response>
    /// <response code="400">Email o contraseña inválidos</response>
    /// <response code="401">Credenciales incorrectas</response>
    /// <response code="429">Demasiados intentos de login</response>
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Validation failed",
                    detail: "Email and password are required");
            }

            // TODO: Implementar verificación de credenciales contra base de datos
            // Este es un placeholder que debe conectarse a la tabla de usuarios
            var userId = 1; // Placeholder
            var email = request.Email;
            var roles = new List<string> { "member" };

            var accessToken = await _authService.GenerateAccessTokenAsync(userId, email, roles, cancellationToken);
            var refreshToken = await _authService.GenerateRefreshTokenAsync(userId, cancellationToken);

            _logger.LogInformation("User logged in: email={Email}, userId={UserId}", email, userId);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 3600,
                User = new UserDto
                {
                    Id = userId,
                    Email = email,
                    FirstName = "User",
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    Roles = roles
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for email: {Email}", request.Email);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Login failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Registración de nuevo usuario
    /// Rate limited: 5 intentos/minuto por IP
    /// </summary>
    /// <param name="request">Email, contraseña y datos personales</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Usuario creado con access token</returns>
    /// <response code="201">Registro exitoso</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="409">Email ya existe</response>
    /// <response code="429">Demasiados intentos de registro</response>
    [HttpPost("signup")]
    [EnableRateLimiting("signup")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponse>> SignUpAsync(
        [FromBody] SignUpRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Validation failed",
                    detail: "Invalid request data");
            }

            // TODO: Implementar creación de usuario
            // 1. Verificar que email no existe
            // 2. Hash de contraseña
            // 3. Crear usuario en DB
            // 4. Enviar email de verificación

            var userId = 2; // Placeholder
            var roles = new List<string> { "member" };

            var accessToken = await _authService.GenerateAccessTokenAsync(userId, request.Email, roles, cancellationToken);
            var refreshToken = await _authService.GenerateRefreshTokenAsync(userId, cancellationToken);
            var emailVerifyToken = await _authService.GenerateEmailVerificationTokenAsync(userId, cancellationToken);

            // TODO: Enviar email de verificación con emailVerifyToken

            _logger.LogInformation("User signed up: email={Email}, userId={UserId}", request.Email, userId);

            return Created($"/api/v1/auth/profile/{userId}", new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 3600,
                User = new UserDto
                {
                    Id = userId,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailVerified = false, // Requiere verificación
                    CreatedAt = DateTime.UtcNow,
                    Roles = roles
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Signup error: {Message}", ex.Message);
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Signup failed",
                detail: ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signup error for email: {Email}", request.Email);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Signup failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Renovar access token usando refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Nuevo access token</returns>
    /// <response code="200">Token renovado</response>
    /// <response code="400">Refresh token inválido o expirado</response>
    /// <response code="401">No autorizado</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (isValid, userId) = await _authService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (!isValid)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid token",
                    detail: "Refresh token is invalid or expired");
            }

            // TODO: Obtener usuario de DB para actualizar roles/email
            var email = "user@example.com";
            var roles = new List<string> { "member" };

            var accessToken = await _authService.GenerateAccessTokenAsync(userId, email, roles, cancellationToken);
            var newRefreshToken = await _authService.GenerateRefreshTokenAsync(userId, cancellationToken);

            _logger.LogInformation("Token refreshed for user: userId={UserId}", userId);

            return Ok(new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Token refresh failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Logout (revocar refresh token)
    /// </summary>
    /// <param name="request">Refresh token a revocar</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Mensaje de éxito</returns>
    /// <response code="200">Logout exitoso</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> LogoutAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            _logger.LogInformation("User logged out");

            return Ok(new MessageResponse
            {
                Message = "Logout successful",
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Logout failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Solicitar reset de contraseña
    /// </summary>
    /// <param name="request">Email del usuario</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmación de envío</returns>
    /// <response code="200">Email de reset enviado</response>
    /// <response code="400">Email inválido</response>
    [HttpPost("forgot-password")]
    [EnableRateLimiting("forgotPassword")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageResponse>> ForgotPasswordAsync(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Buscar usuario por email en DB
            // Si existe, generar token y enviar email
            // Responder OK sin revelar si existe o no (security)

            var userId = 1; // Placeholder
            var resetToken = await _authService.GeneratePasswordResetTokenAsync(userId, cancellationToken);

            // TODO: Enviar email con resetToken
            // El link sería: https://frontend.com/reset-password?token={resetToken}&email={email}

            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

            return Ok(new MessageResponse
            {
                Message = "If an account exists with that email, you will receive password reset instructions",
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Request failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Resetear contraseña con token válido
    /// </summary>
    /// <param name="request">Token de reset + nueva contraseña</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmación de éxito</returns>
    /// <response code="200">Contraseña actualizada</response>
    /// <response code="400">Token inválido o expirado</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageResponse>> ResetPasswordAsync(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (isValid, userId) = await _authService.ValidatePasswordResetTokenAsync(request.Token, cancellationToken);

            if (!isValid)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid token",
                    detail: "Password reset token is invalid or expired");
            }

            // TODO: Actualizar contraseña en DB
            // 1. Hash nueva contraseña
            // 2. Guardar en usuario

            _logger.LogInformation("Password reset for user: userId={UserId}", userId);

            return Ok(new MessageResponse
            {
                Message = "Password has been reset successfully",
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Reset failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Verificar email del usuario
    /// </summary>
    /// <param name="request">Token de verificación</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmación de email verificado</returns>
    /// <response code="200">Email verificado</response>
    /// <response code="400">Token inválido o expirado</response>
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VerifyEmailResponse>> VerifyEmailAsync(
        [FromBody] VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (isValid, userId) = await _authService.ValidateEmailVerificationTokenAsync(request.Token, cancellationToken);

            if (!isValid)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid token",
                    detail: "Email verification token is invalid or expired");
            }

            // TODO: Marcar email como verificado en DB

            _logger.LogInformation("Email verified for user: userId={UserId}", userId);

            return Ok(new VerifyEmailResponse
            {
                Message = "Email verified successfully",
                EmailVerified = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email verification error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Verification failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Obtener perfil del usuario autenticado actual
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Datos del perfil del usuario</returns>
    /// <response code="200">Perfil obtenido</response>
    /// <response code="401">No autenticado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Obtener userId del claim en token
            // TODO: Obtener datos del usuario de DB
            var userId = 1; // Placeholder

            var response = new UserProfileResponse
            {
                Id = userId,
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow,
                EmailVerified = true,
                Roles = new List<string> { "member" }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get profile error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get profile failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Cambiar contraseña del usuario autenticado
    /// </summary>
    /// <param name="request">Contraseña actual y nueva</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmación de cambio</returns>
    /// <response code="200">Contraseña cambiada</response>
    /// <response code="400">Contraseña actual incorrecta</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageResponse>> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Obtener userId del token
            // TODO: Verificar contraseña actual
            // TODO: Hash nueva contraseña y guardar

            _logger.LogInformation("Password changed for user");

            return Ok(new MessageResponse
            {
                Message = "Password changed successfully",
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Change password error");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Change password failed",
                detail: "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Login con OAuth token (Google o GitHub)
    /// </summary>
    /// <param name="request">ID token y provider</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Access token + refresh token</returns>
    /// <response code="200">Login exitoso</response>
    /// <response code="400">Token inválido</response>
    [HttpPost("oauth/token")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(OAuthExchangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OAuthExchangeResponse>> OAuthTokenAsync(
        [FromBody] OAuthTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementar validación de OAuth tokens
            // 1. Validar token con proveedor (Google/GitHub)
            // 2. Obtener perfil del usuario
            // 3. Buscar o crear usuario en DB
            // 4. Generar JWT tokens

            var userId = 3; // Placeholder
            var email = "oauth@example.com";
            var roles = new List<string> { "member" };

            var accessToken = await _authService.GenerateAccessTokenAsync(userId, email, roles, cancellationToken);
            var refreshToken = await _authService.GenerateRefreshTokenAsync(userId, cancellationToken);

            _logger.LogInformation("OAuth login: provider={Provider}, email={Email}", request.Provider, email);

            return Ok(new OAuthExchangeResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsNewUser = false,
                User = new UserDto
                {
                    Id = userId,
                    Email = email,
                    FirstName = "OAuth User",
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    Roles = roles
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OAuth token error: provider={Provider}", request.Provider);
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "OAuth login failed",
                detail: "Invalid or expired OAuth token");
        }
    }

    #region App Session Endpoints (Entra ID Exchange + Refresh Token Rotation)

    /// <summary>
    /// Intercambia un token de Entra ID por una sesión de la aplicación
    /// Este es el endpoint PRINCIPAL para autenticación en producción
    /// </summary>
    /// <param name="request">Token de Entra ID (id_token o access_token)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Access token de la aplicación y datos del usuario</returns>
    /// <response code="200">Exchange exitoso, sesión creada</response>
    /// <response code="400">Token inválido o mal formado</response>
    /// <response code="401">Token expirado o no válido</response>
    [HttpPost("exchange")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(AuthSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthSessionResponse>> ExchangeEntraTokenAsync(
        [FromBody] ExchangeTokenRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Request",
                detail: "IdToken is required");
        }

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var response = await _authSessionService.ExchangeEntraTokenAsync(
                request.IdToken,
                ipAddress,
                userAgent,
                cancellationToken);

            // Emitir refresh token como cookie httpOnly
            var (refreshToken, expiresAt) = await _authSessionService.IssueRefreshTokenAsync(
                response.User.Id,
                ipAddress,
                userAgent,
                cancellationToken);

            SetRefreshTokenCookie(refreshToken, expiresAt);

            _logger.LogInformation(
                "Entra token exchange successful for user {Email}",
                response.User.Email);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token exchange failed: {Message}", ex.Message);
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token exchange");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Refresca el access token usando el refresh token rotativo en cookie
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Nuevo access token y datos del usuario</returns>
    /// <response code="200">Token refrescado exitosamente</response>
    /// <response code="401">Refresh token inválido, expirado o revocado</response>
    [HttpPost("refresh-session")]
    [ProducesResponseType(typeof(AuthSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthSessionResponse>> RefreshSessionAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "Refresh token not found");
        }

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var (newRefreshToken, newAccessToken, refreshExpiresAt) = await _authSessionService.RotateRefreshTokenAsync(
                refreshToken,
                ipAddress,
                userAgent,
                cancellationToken);

            // Obtener info del usuario
            var (isValid, identityUserId) = await _authSessionService.ValidateRefreshTokenAsync(
                newRefreshToken,
                cancellationToken);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Failed to validate new refresh token");
            }

            var userInfo = await _authSessionService.GetUserInfoAsync(identityUserId, cancellationToken);
            if (userInfo == null)
            {
                throw new InvalidOperationException("Failed to retrieve user info");
            }

            // Actualizar cookie con nuevo refresh token
            SetRefreshTokenCookie(newRefreshToken, refreshExpiresAt);

            var accessTokenLifetime = int.Parse(_configuration["Jwt:AccessTokenLifetimeMinutes"] ?? "15");

            var response = new AuthSessionResponse
            {
                AccessToken = newAccessToken,
                TokenType = "Bearer",
                ExpiresIn = accessTokenLifetime * 60,
                User = userInfo
            };

            _logger.LogInformation(
                "Session refreshed for IdentityUserId: {UserId}",
                identityUserId);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed: {Message}", ex.Message);
            ClearRefreshTokenCookie();
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: ex.Message);
        }
        catch (Infrastructure.Services.SecurityException ex)
        {
            _logger.LogWarning(ex, "Security violation during token refresh: {Message}", ex.Message);
            ClearRefreshTokenCookie();
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Security Violation",
                detail: ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Cierra la sesión del usuario revocando su refresh token rotativo
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmación de logout</returns>
    /// <response code="200">Logout exitoso</response>
    [HttpPost("logout-session")]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LogoutResponse>> LogoutSessionAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            try
            {
                await _authSessionService.RevokeRefreshTokenAsync(
                    refreshToken,
                    "User logout",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token during logout");
                // Continuar con logout aunque falle la revocación
            }
        }

        ClearRefreshTokenCookie();

        _logger.LogInformation("User logged out (session revoked)");

        return Ok(new LogoutResponse { Message = "Logged out successfully" });
    }

    /// <summary>
    /// Obtiene información del usuario autenticado actual (desde access token de app)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Información del usuario</returns>
    /// <response code="200">Usuario autenticado</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfo>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var identityUserId))
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "User ID not found in token");
        }

        var userInfo = await _authSessionService.GetUserInfoAsync(identityUserId, cancellationToken);

        if (userInfo == null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: "User not found");
        }

        return Ok(userInfo);
    }

    #endregion

    #region Private Helpers

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS only
            SameSite = SameSiteMode.Strict, // CSRF protection
            Expires = expiresAt,
            Path = "/",
            IsEssential = true
        };

        // En desarrollo, permitir cookies sin HTTPS
        if (_configuration.GetValue<bool>("Development:AllowInsecureCookies"))
        {
            cookieOptions.Secure = false;
        }

        Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
    }

    #endregion
}
