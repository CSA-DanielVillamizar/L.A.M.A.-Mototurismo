using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Lama.Application.Abstractions;

namespace Lama.Application.Services;

/// <summary>
/// Interfaz para servicio de autenticación local (email/password)
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Genera un JWT token para un usuario
    /// </summary>
    Task<string> GenerateAccessTokenAsync(int userId, string email, List<string> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un refresh token
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida un refresh token
    /// </summary>
    Task<(bool IsValid, int UserId)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca un refresh token (logout)
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera token de verificación de email
    /// </summary>
    Task<string> GenerateEmailVerificationTokenAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida token de verificación de email
    /// </summary>
    Task<(bool IsValid, int UserId)> ValidateEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera token de reset de contraseña
    /// </summary>
    Task<string> GeneratePasswordResetTokenAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida token de reset de contraseña
    /// </summary>
    Task<(bool IsValid, int UserId)> ValidatePasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
}

/// <summary>
/// Servicio de autenticación local con JWT
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly ICacheService _cacheService;

    public AuthenticationService(
        IConfiguration configuration,
        ILogger<AuthenticationService> logger,
        ICacheService cacheService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<string> GenerateAccessTokenAsync(
        int userId,
        string email,
        List<string> roles,
        CancellationToken cancellationToken = default)
    {
        var key = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey not configured");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, email),
        };

        // Agregar roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Agregar scopes/permisos según rol
        var scopes = GetScopesForRoles(roles);
        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var issuer = _configuration["Jwt:Issuer"] ?? "lama-api";
        var audience = _configuration["Jwt:Audience"] ?? "lama-app";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(token);

        _logger.LogInformation(
            "Access token generated for user: userId={UserId}, email={Email}, expiresAt={ExpiresAt}",
            userId, email, token.ValidTo);

        return await Task.FromResult(accessToken);
    }

    public async Task<string> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Generar token único aleatorio
        var refreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

        // Cachear refresh token con TTL de 7 días
        var cacheKey = $"refresh_token:{refreshToken}";
        var expirationHours = 24 * 7; // 7 days

        await _cacheService.SetAsync(
            cacheKey,
            userId.ToString(),
            TimeSpan.FromHours(expirationHours),
            cancellationToken);

        _logger.LogInformation(
            "Refresh token generated for user: userId={UserId}, expiresIn={Hours}h",
            userId, expirationHours);

        return refreshToken;
    }

    public async Task<(bool IsValid, int UserId)> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"refresh_token:{refreshToken}";
        var userIdStr = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("Invalid or expired refresh token: {RefreshToken}", refreshToken);
            return (false, 0);
        }

        return (true, userId);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"refresh_token:{refreshToken}";
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Refresh token revoked: {RefreshToken}", refreshToken);
    }

    public async Task<string> GenerateEmailVerificationTokenAsync(int userId, CancellationToken cancellationToken = default)
    {
        var token = GenerateSecureToken(32);
        var cacheKey = $"email_verify_token:{token}";

        await _cacheService.SetAsync(
            cacheKey,
            userId.ToString(),
            TimeSpan.FromHours(24), // Token válido por 24 horas
            cancellationToken);

        return token;
    }

    public async Task<(bool IsValid, int UserId)> ValidateEmailVerificationTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"email_verify_token:{token}";
        var userIdStr = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("Invalid or expired email verification token");
            return (false, 0);
        }

        // Eliminar token después de validarlo (single use)
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        return (true, userId);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(int userId, CancellationToken cancellationToken = default)
    {
        var token = GenerateSecureToken(32);
        var cacheKey = $"password_reset_token:{token}";

        await _cacheService.SetAsync(
            cacheKey,
            userId.ToString(),
            TimeSpan.FromHours(1), // Token válido por 1 hora
            cancellationToken);

        return token;
    }

    public async Task<(bool IsValid, int UserId)> ValidatePasswordResetTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"password_reset_token:{token}";
        var userIdStr = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("Invalid or expired password reset token");
            return (false, 0);
        }

        // Eliminar token después de validarlo (single use)
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        return (true, userId);
    }

    private static string GenerateSecureToken(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static List<string> GetScopesForRoles(List<string> roles)
    {
        var scopes = new List<string>();

        foreach (var role in roles)
        {
            scopes.AddRange(role.ToLower() switch
            {
                "admin" => new[] { "read:all", "write:all", "manage:users", "manage:events" },
                "moderator" => new[] { "read:all", "write:evidence", "manage:events" },
                "member" => new[] { "read:self", "write:evidence" },
                _ => new[] { "read:public" }
            });
        }

        return scopes.Distinct().ToList();
    }
}
