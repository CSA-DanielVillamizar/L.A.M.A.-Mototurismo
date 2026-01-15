using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lama.Domain.Entities;
using Lama.Application.Services;
using Lama.Application.Abstractions;
using Lama.Infrastructure.Data;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación de IIdentityUserService
/// Sincroniza identidades de Entra ID con usuarios locales de LAMA
/// </summary>
public class IdentityUserService : IIdentityUserService
{
    private readonly LamaDbContext _dbContext;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<IdentityUserService> _logger;

    public IdentityUserService(
        LamaDbContext dbContext,
        ITenantProvider tenantProvider,
        ILogger<IdentityUserService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea o actualiza un IdentityUser basado en claims de Entra ID
    /// Se ejecuta en cada request autenticado para sincronizar LastLoginAt
    /// </summary>
    public async Task<IdentityUser> EnsureIdentityUserAsync(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        // Extraer claims de Entra ID
        var subjectId = claimsPrincipal.FindFirst("sub")?.Value 
            ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = claimsPrincipal.FindFirst("email")?.Value 
            ?? claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var displayName = claimsPrincipal.FindFirst("name")?.Value 
            ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(subjectId) || string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                "Claims 'sub' (NameIdentifier) y 'email' son requeridos de Entra ID");
        }

        var tenantId = _tenantProvider.CurrentTenantId;

        // Buscar IdentityUser existente
        var identityUser = await _dbContext.IdentityUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                iu => iu.ExternalSubjectId == subjectId && iu.TenantId == tenantId,
                cancellationToken);

        if (identityUser == null)
        {
            // Crear nuevo
            identityUser = new IdentityUser
            {
                TenantId = tenantId,
                ExternalSubjectId = subjectId,
                Email = email,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.IdentityUsers.Add(identityUser);
            _logger.LogInformation(
                "Created new IdentityUser: {Email} (ExternalSubjectId: {SubjectId})",
                email, subjectId);
        }
        else
        {
            // Actualizar LastLoginAt
            identityUser = new IdentityUser
            {
                Id = identityUser.Id,
                TenantId = tenantId,
                ExternalSubjectId = subjectId,
                Email = email,
                DisplayName = displayName,
                CreatedAt = identityUser.CreatedAt,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MemberId = identityUser.MemberId,
                IsActive = identityUser.IsActive
            };

            _dbContext.IdentityUsers.Update(identityUser);
            _logger.LogDebug(
                "Updated IdentityUser LastLoginAt: {Email}",
                email);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return identityUser;
    }

    /// <summary>
    /// Vincula un IdentityUser con un miembro de LAMA
    /// Operación de admin
    /// </summary>
    public async Task<IdentityUser> LinkToMemberAsync(
        string externalSubjectId,
        int memberId,
        CancellationToken cancellationToken = default)
    {
        // Validar que el miembro existe
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

        if (member == null)
        {
            throw new ArgumentException($"Miembro con ID {memberId} no encontrado", nameof(memberId));
        }

        var tenantId = _tenantProvider.CurrentTenantId;

        // Buscar IdentityUser
        var identityUser = await _dbContext.IdentityUsers
            .FirstOrDefaultAsync(
                iu => iu.ExternalSubjectId == externalSubjectId && iu.TenantId == tenantId,
                cancellationToken);

        if (identityUser == null)
        {
            throw new ArgumentException(
                $"IdentityUser con ExternalSubjectId {externalSubjectId} no encontrado",
                nameof(externalSubjectId));
        }

        // Actualizar MemberId
        identityUser.MemberId = memberId;
        identityUser.UpdatedAt = DateTime.UtcNow;

        _dbContext.IdentityUsers.Update(identityUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Linked IdentityUser {Email} to Member {MemberId}",
            identityUser.Email, memberId);

        return identityUser;
    }

    /// <summary>
    /// Obtiene el IdentityUser actual desde los claims
    /// </summary>
    public async Task<IdentityUser?> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var subjectId = claimsPrincipal.FindFirst("sub")?.Value 
            ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(subjectId))
            return null;

        var tenantId = _tenantProvider.CurrentTenantId;

        return await _dbContext.IdentityUsers
            .Include(iu => iu.Member)
            .FirstOrDefaultAsync(
                iu => iu.ExternalSubjectId == subjectId && iu.TenantId == tenantId,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene el IdentityUser por ExternalSubjectId
    /// </summary>
    public async Task<IdentityUser?> GetByExternalSubjectIdAsync(
        string externalSubjectId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.CurrentTenantId;

        return await _dbContext.IdentityUsers
            .Include(iu => iu.Member)
            .FirstOrDefaultAsync(
                iu => iu.ExternalSubjectId == externalSubjectId && iu.TenantId == tenantId,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene el IdentityUser por Email
    /// </summary>
    public async Task<IdentityUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.CurrentTenantId;

        return await _dbContext.IdentityUsers
            .Include(iu => iu.Member)
            .FirstOrDefaultAsync(
                iu => iu.Email == email && iu.TenantId == tenantId,
                cancellationToken);
    }
}
