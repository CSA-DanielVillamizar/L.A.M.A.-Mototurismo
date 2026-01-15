namespace Lama.Domain.Entities;

/// <summary>
/// Registro de identidad de usuario vinculado a Entra ID / Azure B2C
/// Sincroniza claims de Entra con datos locales de LAMA
/// </summary>
public class IdentityUser
{
    /// <summary>Identificador único en LAMA</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>Subject ID único de Entra ID / Azure B2C (claim "sub" en JWT)</summary>
    public required string ExternalSubjectId { get; set; }

    /// <summary>Email del usuario (desde claim "email" de Entra ID)</summary>
    public required string Email { get; set; }

    /// <summary>Nombre completo del usuario (desde claim "name")</summary>
    public string? DisplayName { get; set; }

    /// <summary>ID del miembro de LAMA asociado (nullable - puede no estar vinculado aún)</summary>
    public int? MemberId { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Último login registrado</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Fecha de actualización del registro</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Relación: Miembro de LAMA asociado</summary>
    public Member? Member { get; set; }

    /// <summary>Indica si la cuenta está activa</summary>
    public bool IsActive { get; set; } = true;
}
