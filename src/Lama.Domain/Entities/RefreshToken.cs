namespace Lama.Domain.Entities;

/// <summary>
/// Token de refresh para sesiones web rotativas con seguridad mejorada
/// </summary>
public class RefreshToken
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>ID del usuario de identidad</summary>
    public int IdentityUserId { get; set; }

    /// <summary>Hash SHA-256 del token (NO guardar token en claro)</summary>
    public required string TokenHash { get; set; }

    /// <summary>Fecha de expiración del token</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Fecha de revocación (si fue revocado explícitamente)</summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>ID del token que reemplazó a este (rotación)</summary>
    public int? ReplacedByTokenId { get; set; }

    /// <summary>Motivo de la revocación</summary>
    public string? RevocationReason { get; set; }

    /// <summary>Fecha de creación del token</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>IP desde donde se creó el token</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>User Agent del cliente que creó el token</summary>
    public string? UserAgent { get; set; }

    /// <summary>Relación: Usuario de identidad asociado</summary>
    public IdentityUser? IdentityUser { get; set; }

    /// <summary>Indica si el token está activo (no expirado ni revocado)</summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>Indica si el token está expirado</summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>Indica si el token fue revocado</summary>
    public bool IsRevoked => RevokedAt != null;
}
