using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lama.Domain.Entities;
using Lama.Application.Abstractions;
using Lama.Application.Services;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de autorización de usuarios.
/// Gestiona asignación, validación y revocación de roles y scopes.
/// </summary>
public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly ILamaDbContext _dbContext;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<UserAuthorizationService> _logger;

    public UserAuthorizationService(
        ILamaDbContext dbContext,
        ITenantProvider tenantProvider,
        ILogger<UserAuthorizationService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(string externalSubjectId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        _logger.LogDebug("Obteniendo roles del usuario {ExternalSubjectId} en tenant {TenantId}", 
            externalSubjectId, _tenantProvider.CurrentTenantId);

        var roles = await _dbContext.UserRoles
            .Where(ur => ur.ExternalSubjectId == externalSubjectId && ur.TenantId == _tenantProvider.CurrentTenantId)
            .OrderByDescending(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);

        return roles;
    }

    public async Task<IEnumerable<UserScope>> GetUserScopesAsync(string externalSubjectId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        _logger.LogDebug("Obteniendo scopes del usuario {ExternalSubjectId} en tenant {TenantId}",
            externalSubjectId, _tenantProvider.CurrentTenantId);

        var scopes = await _dbContext.UserScopes
            .Where(us => us.ExternalSubjectId == externalSubjectId && us.TenantId == _tenantProvider.CurrentTenantId)
            .OrderByDescending(us => us.AssignedAt)
            .ToListAsync(cancellationToken);

        return scopes;
    }

    public async Task<bool> HasRoleAsync(string externalSubjectId, RoleType role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        var hasRole = await _dbContext.UserRoles
            .AnyAsync(ur =>
                ur.ExternalSubjectId == externalSubjectId &&
                ur.Role == role &&
                ur.IsActive &&
                ur.TenantId == _tenantProvider.CurrentTenantId &&
                (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow),
                cancellationToken);

        _logger.LogDebug("Usuario {ExternalSubjectId} tiene rol {Role}: {HasRole}",
            externalSubjectId, role, hasRole);

        return hasRole;
    }

    public async Task<bool> HasMinimumRoleAsync(string externalSubjectId, RoleType minimumRole, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        var userRole = await _dbContext.UserRoles
            .Where(ur =>
                ur.ExternalSubjectId == externalSubjectId &&
                ur.IsActive &&
                ur.TenantId == _tenantProvider.CurrentTenantId &&
                (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (userRole == null)
        {
            _logger.LogDebug("Usuario {ExternalSubjectId} no tiene rol mínimo {MinimumRole}",
                externalSubjectId, minimumRole);
            return false;
        }

        var hasMinimumRole = userRole.Role >= minimumRole;

        _logger.LogDebug("Usuario {ExternalSubjectId} (rol actual: {UserRole}) cumple rol mínimo {MinimumRole}: {Result}",
            externalSubjectId, userRole.Role, minimumRole, hasMinimumRole);

        return hasMinimumRole;
    }

    public async Task<bool> HasScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        var hasScope = await _dbContext.UserScopes
            .AnyAsync(us =>
                us.ExternalSubjectId == externalSubjectId &&
                us.ScopeType == scopeType &&
                us.ScopeId == scopeId &&
                us.IsActive &&
                us.TenantId == _tenantProvider.CurrentTenantId &&
                (us.ExpiresAt == null || us.ExpiresAt > DateTime.UtcNow),
                cancellationToken);

        _logger.LogDebug("Usuario {ExternalSubjectId} tiene scope {ScopeType}:{ScopeId}: {HasScope}",
            externalSubjectId, scopeType, scopeId ?? "GLOBAL", hasScope);

        return hasScope;
    }

    public async Task<bool> CanAccessResourceAsync(
        string externalSubjectId,
        RoleType requiredRole,
        ScopeType requiredScopeType,
        string? resourceScopeId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        _logger.LogDebug("Verificando acceso de usuario {ExternalSubjectId} a recurso {RequiredScopeType}:{ResourceScopeId}",
            externalSubjectId, requiredScopeType, resourceScopeId ?? "GLOBAL");

        // Verificar rol mínimo
        if (!await HasMinimumRoleAsync(externalSubjectId, requiredRole, cancellationToken))
        {
            _logger.LogWarning("Usuario {ExternalSubjectId} no tiene rol mínimo requerido {RequiredRole}",
                externalSubjectId, requiredRole);
            return false;
        }

        // Verificar scope - obtener scopes del usuario
        var userScopes = await GetUserScopesAsync(externalSubjectId, cancellationToken);
        var activeScopes = userScopes
            .Where(us => us.IsActive && (us.ExpiresAt == null || us.ExpiresAt > DateTime.UtcNow))
            .ToList();

        // SUPER_ADMIN tiene acceso global
        var userRole = await _dbContext.UserRoles
            .Where(ur =>
                ur.ExternalSubjectId == externalSubjectId &&
                ur.IsActive &&
                ur.TenantId == _tenantProvider.CurrentTenantId &&
                (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (userRole?.Role == RoleType.SUPER_ADMIN)
        {
            _logger.LogDebug("Usuario {ExternalSubjectId} es SUPER_ADMIN, acceso concedido", externalSubjectId);
            return true;
        }

        // Verificar si el usuario tiene un scope que cubra el recurso
        var hasAccessScope = activeScopes.Any(us =>
            us.ScopeType == ScopeType.GLOBAL ||
            (us.ScopeType == requiredScopeType && us.ScopeId == resourceScopeId) ||
            (us.ScopeType == ScopeType.CONTINENT && requiredScopeType == ScopeType.COUNTRY) ||
            (us.ScopeType == ScopeType.COUNTRY && requiredScopeType == ScopeType.CHAPTER));

        if (!hasAccessScope)
        {
            _logger.LogWarning("Usuario {ExternalSubjectId} no tiene scope que cubra el recurso {RequiredScopeType}:{ResourceScopeId}",
                externalSubjectId, requiredScopeType, resourceScopeId ?? "GLOBAL");
            return false;
        }

        _logger.LogDebug("Acceso concedido a usuario {ExternalSubjectId} para recurso {RequiredScopeType}:{ResourceScopeId}",
            externalSubjectId, requiredScopeType, resourceScopeId ?? "GLOBAL");

        return true;
    }

    public async Task<UserRole> AssignRoleAsync(
        string externalSubjectId,
        RoleType role,
        string assignedByExternalSubjectId,
        string reason,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        if (string.IsNullOrWhiteSpace(assignedByExternalSubjectId))
            throw new ArgumentException("AssignedByExternalSubjectId no puede estar vacío", nameof(assignedByExternalSubjectId));

        _logger.LogInformation("Asignando rol {Role} a usuario {ExternalSubjectId}. Motivo: {Reason}",
            role, externalSubjectId, reason);

        // Desactivar rol existente del mismo tipo si lo hay
        var existingRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur =>
                ur.ExternalSubjectId == externalSubjectId &&
                ur.Role == role &&
                ur.IsActive &&
                ur.TenantId == _tenantProvider.CurrentTenantId,
                cancellationToken);

        if (existingRole != null)
        {
            existingRole.IsActive = false;
            existingRole.UpdatedAt = DateTime.UtcNow;
            _logger.LogDebug("Rol existente {Role} para usuario {ExternalSubjectId} deactivado",
                role, externalSubjectId);
        }

        var userRole = new UserRole
        {
            TenantId = _tenantProvider.CurrentTenantId,
            ExternalSubjectId = externalSubjectId,
            Role = role,
            AssignedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsActive = true,
            Reason = reason,
            AssignedBy = assignedByExternalSubjectId,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rol {Role} asignado exitosamente a usuario {ExternalSubjectId}",
            role, externalSubjectId);

        return userRole;
    }

    public async Task<UserScope> AssignScopeAsync(
        string externalSubjectId,
        ScopeType scopeType,
        string? scopeId,
        string assignedByExternalSubjectId,
        string reason,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        if (string.IsNullOrWhiteSpace(assignedByExternalSubjectId))
            throw new ArgumentException("AssignedByExternalSubjectId no puede estar vacío", nameof(assignedByExternalSubjectId));

        _logger.LogInformation("Asignando scope {ScopeType}:{ScopeId} a usuario {ExternalSubjectId}. Motivo: {Reason}",
            scopeType, scopeId ?? "GLOBAL", externalSubjectId, reason);

        // Desactivar scope existente del mismo tipo y ID si lo hay
        var existingScope = await _dbContext.UserScopes
            .FirstOrDefaultAsync(us =>
                us.ExternalSubjectId == externalSubjectId &&
                us.ScopeType == scopeType &&
                us.ScopeId == scopeId &&
                us.IsActive &&
                us.TenantId == _tenantProvider.CurrentTenantId,
                cancellationToken);

        if (existingScope != null)
        {
            existingScope.IsActive = false;
            existingScope.UpdatedAt = DateTime.UtcNow;
            _logger.LogDebug("Scope existente {ScopeType}:{ScopeId} para usuario {ExternalSubjectId} deactivado",
                scopeType, scopeId ?? "GLOBAL", externalSubjectId);
        }

        var userScope = new UserScope
        {
            TenantId = _tenantProvider.CurrentTenantId,
            ExternalSubjectId = externalSubjectId,
            ScopeType = scopeType,
            ScopeId = scopeId,
            AssignedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsActive = true,
            Reason = reason,
            AssignedBy = assignedByExternalSubjectId,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.UserScopes.Add(userScope);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scope {ScopeType}:{ScopeId} asignado exitosamente a usuario {ExternalSubjectId}",
            scopeType, scopeId ?? "GLOBAL", externalSubjectId);

        return userScope;
    }

    public async Task<bool> RevokeRoleAsync(string externalSubjectId, RoleType role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        _logger.LogInformation("Revocando rol {Role} del usuario {ExternalSubjectId}", role, externalSubjectId);

        var userRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur =>
                ur.ExternalSubjectId == externalSubjectId &&
                ur.Role == role &&
                ur.IsActive &&
                ur.TenantId == _tenantProvider.CurrentTenantId,
                cancellationToken);

        if (userRole == null)
        {
            _logger.LogWarning("No se encontró rol {Role} activo para usuario {ExternalSubjectId}",
                role, externalSubjectId);
            return false;
        }

        userRole.IsActive = false;
        userRole.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rol {Role} revocado exitosamente del usuario {ExternalSubjectId}",
            role, externalSubjectId);

        return true;
    }

    public async Task<bool> RevokeScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalSubjectId))
            throw new ArgumentException("ExternalSubjectId no puede estar vacío", nameof(externalSubjectId));

        _logger.LogInformation("Revocando scope {ScopeType}:{ScopeId} del usuario {ExternalSubjectId}",
            scopeType, scopeId ?? "GLOBAL", externalSubjectId);

        var userScope = await _dbContext.UserScopes
            .FirstOrDefaultAsync(us =>
                us.ExternalSubjectId == externalSubjectId &&
                us.ScopeType == scopeType &&
                us.ScopeId == scopeId &&
                us.IsActive &&
                us.TenantId == _tenantProvider.CurrentTenantId,
                cancellationToken);

        if (userScope == null)
        {
            _logger.LogWarning("No se encontró scope {ScopeType}:{ScopeId} activo para usuario {ExternalSubjectId}",
                scopeType, scopeId ?? "GLOBAL", externalSubjectId);
            return false;
        }

        userScope.IsActive = false;
        userScope.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scope {ScopeType}:{ScopeId} revocado exitosamente del usuario {ExternalSubjectId}",
            scopeType, scopeId ?? "GLOBAL", externalSubjectId);

        return true;
    }
}
