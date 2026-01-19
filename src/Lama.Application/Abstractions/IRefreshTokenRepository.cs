using Lama.Domain.Entities;

namespace Lama.Application.Abstractions;

/// <summary>
/// Repositorio para gestión de refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Obtiene un token por su hash
    /// </summary>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo refresh token
    /// </summary>
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un refresh token existente
    /// </summary>
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca todos los tokens activos de un usuario
    /// </summary>
    Task RevokeAllUserTokensAsync(int identityUserId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca una cadena de tokens (chain) por detección de reuso
    /// </summary>
    Task RevokeTokenChainAsync(int tokenId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina tokens expirados (limpieza periódica)
    /// </summary>
    Task DeleteExpiredTokensAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tokens activos de un usuario
    /// </summary>
    Task<List<RefreshToken>> GetActiveTokensByUserAsync(int identityUserId, CancellationToken cancellationToken = default);
}
