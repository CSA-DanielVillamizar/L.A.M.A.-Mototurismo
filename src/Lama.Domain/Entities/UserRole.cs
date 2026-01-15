namespace Lama.Domain.Entities;

/// <summary>
/// Define los roles disponibles en el sistema LAMA
/// Cada usuario puede tener múltiples roles (ManyToMany via UserRole table)
/// </summary>
public enum RoleType
{
    /// <summary>Miembro regular - acceso básico a su perfil y eventos</summary>
    MEMBER = 0,

    /// <summary>MTO de un capítulo - gestiona eventos y asistentes de su capítulo</summary>
    MTO_CHAPTER = 1,

    /// <summary>Admin de un capítulo - gestiona usuarios y configuración del capítulo</summary>
    ADMIN_CHAPTER = 2,

    /// <summary>Admin nacional - gestiona múltiples capítulos de un país</summary>
    ADMIN_NATIONAL = 3,

    /// <summary>Admin continental - gestiona múltiples países de un continente</summary>
    ADMIN_CONTINENT = 4,

    /// <summary>Admin internacional - acceso global</summary>
    ADMIN_INTERNATIONAL = 5,

    /// <summary>Super admin - acceso total sin restricciones</summary>
    SUPER_ADMIN = 6
}

/// <summary>
/// Vinculación entre usuario (IdentityUser) y roles que posee
/// Modelo Many-to-Many: Un usuario puede tener múltiples roles
/// </summary>
public class UserRole
{
    /// <summary>Identificador único de la asignación de rol</summary>
    public int Id { get; set; }

    /// <summary>Tenant a que pertenece este rol (multi-tenancy)</summary>
    public required Guid TenantId { get; set; }

    /// <summary>Identificador externo de Entra ID (sub claim) del usuario</summary>
    public required string ExternalSubjectId { get; set; }

    /// <summary>Rol que se asigna al usuario</summary>
    public required RoleType Role { get; set; }

    /// <summary>Cuándo se asignó este rol</summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Cuándo expira este rol (null = nunca expira)</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Si el rol está actualmente activo</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Motivo de la asignación (para auditoría)</summary>
    public string? Reason { get; set; }

    /// <summary>Quién asignó este rol (ExternalSubjectId de admin)</summary>
    public string? AssignedBy { get; set; }

    /// <summary>Timestamp de última actualización</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
