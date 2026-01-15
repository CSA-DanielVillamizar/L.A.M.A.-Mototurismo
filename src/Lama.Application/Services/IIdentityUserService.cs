using System.Security.Claims;
using Lama.Domain.Entities;

namespace Lama.Application.Services;

/// <summary>
/// Servicio para gestionar sincronización entre identidades de Entra ID y usuarios locales
/// </summary>
public interface IIdentityUserService
{
    /// <summary>
    /// Asegura que existe un IdentityUser para los claims dados, creándolo o actualizando LastLoginAt
    /// Se ejecuta después de autenticación exitosa para sincronizar datos
    /// </summary>
    /// <param name="claimsPrincipal">Principal con claims de Entra ID</param>
    /// <returns>IdentityUser creado o actualizado</returns>
    Task<IdentityUser> EnsureIdentityUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vincula un usuario de Entra ID con un miembro de LAMA
    /// Operación de admin para asociar identidades externas con miembros locales
    /// </summary>
    /// <param name="externalSubjectId">Subject ID de Entra ID (claim "sub")</param>
    /// <param name="memberId">ID del miembro de LAMA a vincular</param>
    /// <returns>IdentityUser actualizado</returns>
    Task<IdentityUser> LinkToMemberAsync(string externalSubjectId, int memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el IdentityUser actual desde los claims
    /// </summary>
    Task<IdentityUser?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el IdentityUser por ExternalSubjectId (identificador único de Entra)
    /// </summary>
    Task<IdentityUser?> GetByExternalSubjectIdAsync(string externalSubjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el IdentityUser por Email
    /// </summary>
    Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
