namespace Lama.Domain.Entities;

/// <summary>
/// Define los tipos de scope (alcance) que limitan el acceso a recursos
/// </summary>
public enum ScopeType
{
    /// <summary>Acceso a un capítulo específico (ChapterId)</summary>
    CHAPTER = 0,

    /// <summary>Acceso a un país específico (CountryCode)</summary>
    COUNTRY = 1,

    /// <summary>Acceso a un continente específico (ContinentId)</summary>
    CONTINENT = 2,

    /// <summary>Acceso global sin restricciones territoriales</summary>
    GLOBAL = 3
}

/// <summary>
/// Define los scopes (alcances territoriales) que un usuario puede acceder
/// Controla qué recursos (eventos, capítulos, etc) puede ver/modificar un usuario
/// Ejemplo: MTO de Medellín solo puede acceder a events.ChapterId == Medellín_ChapterId
/// </summary>
public class UserScope
{
    /// <summary>Identificador único del scope</summary>
    public int Id { get; set; }

    /// <summary>Tenant a que pertenece este scope (multi-tenancy)</summary>
    public required Guid TenantId { get; set; }

    /// <summary>Identificador externo de Entra ID (sub claim) del usuario</summary>
    public required string ExternalSubjectId { get; set; }

    /// <summary>Tipo de scope (CHAPTER, COUNTRY, CONTINENT, GLOBAL)</summary>
    public required ScopeType ScopeType { get; set; }

    /// <summary>
    /// ID del recurso scope
    /// - Para CHAPTER: ChapterId (int)
    /// - Para COUNTRY: CountryCode (string, ej: "CO", "AR")
    /// - Para CONTINENT: ContinentId (int)
    /// - Para GLOBAL: "GLOBAL" o null
    /// </summary>
    public string? ScopeId { get; set; }

    /// <summary>Cuándo se asignó este scope</summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Cuándo expira este scope (null = nunca expira)</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Si el scope está actualmente activo</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Motivo de la asignación (para auditoría)</summary>
    public string? Reason { get; set; }

    /// <summary>Quién asignó este scope (ExternalSubjectId de admin)</summary>
    public string? AssignedBy { get; set; }

    /// <summary>Timestamp de última actualización</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
