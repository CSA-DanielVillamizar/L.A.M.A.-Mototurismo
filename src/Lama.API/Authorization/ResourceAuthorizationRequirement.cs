using Microsoft.AspNetCore.Authorization;
using Lama.Domain.Entities;

namespace Lama.API.Authorization;

/// <summary>
/// Requirement para autorización basada en roles y scopes de recursos.
/// Se usa con ScopeAuthorizationHandler para validar que el usuario tiene
/// el rol y scope necesarios para acceder a un recurso específico.
/// </summary>
public class ResourceAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Rol mínimo requerido para acceder al recurso.
    /// </summary>
    public RoleType RequiredRole { get; }

    /// <summary>
    /// Tipo de scope requerido para el recurso.
    /// </summary>
    public ScopeType RequiredScopeType { get; }

    /// <summary>
    /// Inicializa un nuevo instance de ResourceAuthorizationRequirement.
    /// </summary>
    /// <param name="requiredRole">Rol mínimo requerido</param>
    /// <param name="requiredScopeType">Tipo de scope requerido</param>
    public ResourceAuthorizationRequirement(RoleType requiredRole, ScopeType requiredScopeType)
    {
        RequiredRole = requiredRole;
        RequiredScopeType = requiredScopeType;
    }
}
