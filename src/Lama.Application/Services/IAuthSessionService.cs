using Lama.Application.DTOs;

namespace Lama.Application.Services;

/// <summary>
/// Servicio para gestión de sesiones de autenticación con refresh tokens rotativos
/// </summary>
public interface IAuthSessionService
{
    /// <summary>
    /// Intercambia un token de Entra ID por una sesión de la aplicación
    /// </summary>
    Task<AuthSessionResponse> ExchangeEntraTokenAsync(
        string entraIdToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Emite un nuevo refresh token para un usuario
    /// </summary>
    Task<(string RefreshToken, DateTime ExpiresAt)> IssueRefreshTokenAsync(
        int identityUserId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rota un refresh token (invalida el actual y emite uno nuevo)
    /// </summary>
    Task<(string NewRefreshToken, string NewAccessToken, DateTime RefreshExpiresAt)> RotateRefreshTokenAsync(
        string currentRefreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida un refresh token
    /// </summary>
    Task<(bool IsValid, int IdentityUserId)> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca un refresh token específico (logout)
    /// </summary>
    Task RevokeRefreshTokenAsync(
        string refreshToken,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca todos los refresh tokens de un usuario
    /// </summary>
    Task RevokeAllUserTokensAsync(
        int identityUserId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un access token de la aplicación (JWT interno)
    /// </summary>
    Task<string> GenerateAppAccessTokenAsync(
        int identityUserId,
        int? memberId,
        string email,
        List<string> roles,
        List<string> scopes,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene información del usuario por ID
    /// </summary>
    Task<UserInfo?> GetUserInfoAsync(int identityUserId, CancellationToken cancellationToken = default);
}
