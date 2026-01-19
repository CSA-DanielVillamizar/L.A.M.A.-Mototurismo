using Microsoft.EntityFrameworkCore;
using Lama.Application.Abstractions;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;

namespace Lama.Infrastructure.Repositories;

/// <summary>
/// Repositorio para gestión de refresh tokens
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly LamaDbContext _context;

    public RefreshTokenRepository(LamaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(r => r.IdentityUser)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(int identityUserId, string reason, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.IdentityUserId == identityUserId && r.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevocationReason = reason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenChainAsync(int tokenId, string reason, CancellationToken cancellationToken = default)
    {
        // Revocar el token y todos los que lo reemplazaron (chain)
        var token = await _context.RefreshTokens.FindAsync(new object[] { tokenId }, cancellationToken);
        if (token == null) return;

        // Revocar token actual
        token.RevokedAt = DateTime.UtcNow;
        token.RevocationReason = reason;

        // Buscar y revocar toda la cadena
        var chainTokens = await _context.RefreshTokens
            .Where(r => r.ReplacedByTokenId == tokenId && r.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var chainToken in chainTokens)
        {
            chainToken.RevokedAt = DateTime.UtcNow;
            chainToken.RevocationReason = reason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExpiredTokensAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(r => r.ExpiresAt < olderThan)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserAsync(int identityUserId, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Where(r => r.IdentityUserId == identityUserId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
