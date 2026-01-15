using Lama.Domain.Entities;

namespace Lama.Application.Services;

/// <summary>
/// Interfaz para servicios de autorización de usuarios.
/// Define operaciones para gestionar y validar roles y scopes de usuarios.
/// </summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// Obtiene todos los roles activos asignados a un usuario.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario (sub claim de JWT)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de roles activos del usuario. Vacía si no tiene roles.</returns>
    Task<IEnumerable<UserRole>> GetUserRolesAsync(string externalSubjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los scopes activos asignados a un usuario.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de scopes activos del usuario. Vacía si no tiene scopes.</returns>
    Task<IEnumerable<UserScope>> GetUserScopesAsync(string externalSubjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un usuario tiene un rol específico.
    /// Considera: ExpiresAt, IsActive, fecha actual.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="role">Rol a verificar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el usuario tiene el rol activo; false en caso contrario</returns>
    Task<bool> HasRoleAsync(string externalSubjectId, RoleType role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un usuario tiene un rol igual o superior al especificado.
    /// Utiliza la jerarquía de roles (MEMBER &lt; MTO_CHAPTER &lt; ... &lt; SUPER_ADMIN).
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="minimumRole">Rol mínimo requerido</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el usuario tiene rol &gt;= minimumRole; false en caso contrario</returns>
    Task<bool> HasMinimumRoleAsync(string externalSubjectId, RoleType minimumRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un usuario tiene un scope específico.
    /// Considera: ExpiresAt, IsActive, fecha actual.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="scopeType">Tipo de scope a verificar</param>
    /// <param name="scopeId">Identificador del scope (ChapterId, CountryCode, ContinentId, o null para GLOBAL)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el usuario tiene el scope activo; false en caso contrario</returns>
    Task<bool> HasScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un usuario tiene acceso a un recurso específico basado en rol y scope.
    /// Valida que el usuario tenga el rol requerido y que su scope cubra el recurso.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="requiredRole">Rol mínimo requerido</param>
    /// <param name="requiredScopeType">Tipo de scope requerido</param>
    /// <param name="resourceScopeId">Identificador del scope del recurso (ChapterId, CountryCode, etc.)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el usuario tiene acceso al recurso; false en caso contrario</returns>
    Task<bool> CanAccessResourceAsync(string externalSubjectId, RoleType requiredRole, ScopeType requiredScopeType, string? resourceScopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asigna un rol a un usuario con auditoría.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="role">Rol a asignar</param>
    /// <param name="assignedByExternalSubjectId">Identificador del usuario que realiza la asignación</param>
    /// <param name="reason">Motivo de la asignación (para auditoría)</param>
    /// <param name="expiresAt">Fecha de expiración del rol (null = no expira)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El UserRole creado</returns>
    Task<UserRole> AssignRoleAsync(
        string externalSubjectId,
        RoleType role,
        string assignedByExternalSubjectId,
        string reason,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asigna un scope a un usuario con auditoría.
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="scopeType">Tipo de scope a asignar</param>
    /// <param name="scopeId">Identificador del scope</param>
    /// <param name="assignedByExternalSubjectId">Identificador del usuario que realiza la asignación</param>
    /// <param name="reason">Motivo de la asignación</param>
    /// <param name="expiresAt">Fecha de expiración del scope (null = no expira)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El UserScope creado</returns>
    Task<UserScope> AssignScopeAsync(
        string externalSubjectId,
        ScopeType scopeType,
        string? scopeId,
        string assignedByExternalSubjectId,
        string reason,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca un rol deactivándolo (sin eliminar registro de auditoría).
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="role">Rol a revocar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se revocó exitosamente; false si no existía</returns>
    Task<bool> RevokeRoleAsync(string externalSubjectId, RoleType role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca un scope deactivándolo (sin eliminar registro de auditoría).
    /// </summary>
    /// <param name="externalSubjectId">Identificador externo del usuario</param>
    /// <param name="scopeType">Tipo de scope a revocar</param>
    /// <param name="scopeId">Identificador del scope a revocar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se revocó exitosamente; false si no existía</returns>
    Task<bool> RevokeScopeAsync(string externalSubjectId, ScopeType scopeType, string? scopeId, CancellationToken cancellationToken = default);
}
