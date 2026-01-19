using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Lama.Application.Abstractions;
using Lama.Application.DTOs;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Servicio para gestión de sesiones de autenticación con refresh tokens rotativos
/// </summary>
public class AuthSessionService : Application.Services.IAuthSessionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthSessionService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly Application.Services.IIdentityUserService _identityUserService;
    private readonly ITenantProvider _tenantProvider;

    public AuthSessionService(
        IConfiguration configuration,
        ILogger<AuthSessionService> logger,
        IRefreshTokenRepository refreshTokenRepository,
        Application.Services.IIdentityUserService identityUserService,
        ITenantProvider tenantProvider)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _identityUserService = identityUserService ?? throw new ArgumentNullException(nameof(identityUserService));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
    }

    public async Task<AuthSessionResponse> ExchangeEntraTokenAsync(
        string entraIdToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        // Validar y decodificar el token de Entra ID
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(entraIdToken);

        var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(email))
        {
            throw new UnauthorizedAccessException("Invalid Entra ID token: missing sub or email claims");
        }

        // Crear o actualizar IdentityUser
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
        var identityUser = await _identityUserService.EnsureIdentityUserAsync(claimsPrincipal, cancellationToken);

        // Obtener roles y scopes del usuario
        var userInfo = await GetUserInfoAsync(identityUser.Id, cancellationToken);
        if (userInfo == null)
        {
            throw new InvalidOperationException($"Failed to retrieve user info for IdentityUserId: {identityUser.Id}");
        }

        // Generar access token de la aplicación
        var accessToken = await GenerateAppAccessTokenAsync(
            identityUser.Id,
            identityUser.MemberId,
            email,
            userInfo.Roles,
            userInfo.Scopes,
            identityUser.TenantId,
            cancellationToken);

        // Generar refresh token
        var (refreshToken, expiresAt) = await IssueRefreshTokenAsync(
            identityUser.Id,
            ipAddress,
            userAgent,
            cancellationToken);

        // El refresh token se enviará como cookie httpOnly desde el controller
        _logger.LogInformation(
            "Token exchange successful for user {Email} (IdentityUserId: {UserId})",
            email, identityUser.Id);

        var accessTokenLifetime = int.Parse(_configuration["Jwt:AccessTokenLifetimeMinutes"] ?? "15");

        return new AuthSessionResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = accessTokenLifetime * 60,
            User = userInfo
        };
    }

    public async Task<(string RefreshToken, DateTime ExpiresAt)> IssueRefreshTokenAsync(
        int identityUserId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        // Generar token aleatorio criptográficamente seguro
        var tokenBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        // Hash del token para almacenamiento
        var tokenHash = ComputeTokenHash(token);

        var refreshTokenLifetimeDays = int.Parse(_configuration["Jwt:RefreshTokenLifetimeDays"] ?? "7");
        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays);

        var refreshToken = new RefreshToken
        {
            TenantId = _tenantProvider.CurrentTenantId,
            IdentityUserId = identityUserId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserAgent = userAgent
        };

        await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

        _logger.LogInformation(
            "Refresh token issued for IdentityUserId: {UserId}, expires at {ExpiresAt}",
            identityUserId, expiresAt);

        return (token, expiresAt);
    }

    public async Task<(string NewRefreshToken, string NewAccessToken, DateTime RefreshExpiresAt)> RotateRefreshTokenAsync(
        string currentRefreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeTokenHash(currentRefreshToken);
        var oldToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (oldToken == null)
        {
            _logger.LogWarning("Refresh token not found for rotation");
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Detección de reuso: si el token ya fue revocado, es un intento de reuso
        if (oldToken.IsRevoked)
        {
            _logger.LogWarning(
                "Refresh token reuse detected for IdentityUserId: {UserId}. Revoking token chain.",
                oldToken.IdentityUserId);

            await _refreshTokenRepository.RevokeTokenChainAsync(
                oldToken.Id,
                "Token reuse detected",
                cancellationToken);

            throw new SecurityException("Token reuse detected. Session revoked for security.");
        }

        // Validar expiración
        if (oldToken.IsExpired)
        {
            _logger.LogWarning("Expired refresh token used for IdentityUserId: {UserId}", oldToken.IdentityUserId);
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Validar que el token está activo
        if (!oldToken.IsActive)
        {
            _logger.LogWarning("Inactive refresh token used for IdentityUserId: {UserId}", oldToken.IdentityUserId);
            throw new UnauthorizedAccessException("Refresh token is not active");
        }

        // Generar nuevo refresh token
        var (newRefreshToken, newExpiresAt) = await IssueRefreshTokenAsync(
            oldToken.IdentityUserId,
            ipAddress,
            userAgent,
            cancellationToken);

        // Revocar token anterior y vincularlo al nuevo (cadena)
        var newTokenHash = ComputeTokenHash(newRefreshToken);
        var newTokenEntity = await _refreshTokenRepository.GetByTokenHashAsync(newTokenHash, cancellationToken);

        if (newTokenEntity != null)
        {
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByTokenId = newTokenEntity.Id;
            oldToken.RevocationReason = "Rotated";
            await _refreshTokenRepository.UpdateAsync(oldToken, cancellationToken);
        }

        // Obtener info del usuario para generar nuevo access token
        var userInfo = await GetUserInfoAsync(oldToken.IdentityUserId, cancellationToken);
        if (userInfo == null)
        {
            throw new InvalidOperationException($"Failed to retrieve user info for IdentityUserId: {oldToken.IdentityUserId}");
        }

        var newAccessToken = await GenerateAppAccessTokenAsync(
            oldToken.IdentityUserId,
            userInfo.MemberId,
            userInfo.Email,
            userInfo.Roles,
            userInfo.Scopes,
            userInfo.TenantId,
            cancellationToken);

        _logger.LogInformation(
            "Refresh token rotated for IdentityUserId: {UserId}",
            oldToken.IdentityUserId);

        return (newRefreshToken, newAccessToken, newExpiresAt);
    }

    public async Task<(bool IsValid, int IdentityUserId)> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var token = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (token == null || !token.IsActive)
        {
            return (false, 0);
        }

        return (true, token.IdentityUserId);
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var token = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (token == null) return;

        token.RevokedAt = DateTime.UtcNow;
        token.RevocationReason = reason;
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

        _logger.LogInformation(
            "Refresh token revoked for IdentityUserId: {UserId}, reason: {Reason}",
            token.IdentityUserId, reason);
    }

    public async Task RevokeAllUserTokensAsync(
        int identityUserId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(identityUserId, reason, cancellationToken);

        _logger.LogInformation(
            "All refresh tokens revoked for IdentityUserId: {UserId}, reason: {Reason}",
            identityUserId, reason);
    }

    public async Task<string> GenerateAppAccessTokenAsync(
        int identityUserId,
        int? memberId,
        string email,
        List<string> roles,
        List<string> scopes,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var key = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey not configured");

        var issuer = _configuration["Jwt:Issuer"] ?? "lama-app";
        var audience = _configuration["Jwt:Audience"] ?? "lama-api";
        var lifetimeMinutes = int.Parse(_configuration["Jwt:AccessTokenLifetimeMinutes"] ?? "15");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identityUserId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, email),
            new("tenant_id", tenantId.ToString())
        };

        if (memberId.HasValue)
        {
            claims.Add(new Claim("member_id", memberId.Value.ToString()));
        }

        // Agregar roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("roles", role)); // Compatibilidad con frontend
        }

        // Agregar scopes (permisos de API)
        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scp", scope));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(lifetimeMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<UserInfo?> GetUserInfoAsync(int identityUserId, CancellationToken cancellationToken = default)
    {
        // Aquí deberíamos cargar desde repositorio con joins, por ahora simplificado
        // TODO: Crear método en IIdentityUserService para cargar con roles/scopes/member
        var user = await _identityUserService.GetByExternalSubjectIdAsync(
            identityUserId.ToString(), // Temporal, necesita ajuste
            cancellationToken);

        if (user == null) return null;

        // TODO: Cargar roles y scopes reales desde UserRoles/UserScopes
        var roles = new List<string> { "MEMBER" }; // Placeholder
        var scopes = new List<string> { "api.read", "api.write" }; // Placeholder

        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            MemberId = user.MemberId,
            MemberName = user.Member?.CompleteNames,
            ChapterName = null, // TODO: cargar desde Member.Chapter
            Roles = roles,
            Scopes = scopes,
            TenantId = user.TenantId
        };
    }

    private string ComputeTokenHash(string token)
    {
        var secret = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey not configured");
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
}
