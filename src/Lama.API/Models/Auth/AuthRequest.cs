using System.ComponentModel.DataAnnotations;

namespace Lama.API.Models.Auth;

/// <summary>
/// Solicitud de login con email y contraseña
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Contraseña es requerida")]
    [MinLength(6, ErrorMessage = "Contraseña debe tener al menos 6 caracteres")]
    public required string Password { get; init; }

    [Display(Name = "Recuérdame")]
    public bool RememberMe { get; init; } = false;
}

/// <summary>
/// Solicitud de registro/signup
/// </summary>
public class SignUpRequest
{
    [Required(ErrorMessage = "Email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Contraseña es requerida")]
    [MinLength(8, ErrorMessage = "Contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Contraseña debe contener mayúscula, minúscula, número y carácter especial")]
    public required string Password { get; init; }

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100)]
    public required string FirstName { get; init; }

    [StringLength(100)]
    public string? LastName { get; init; }

    [Required(ErrorMessage = "Aceptar términos es obligatorio")]
    public bool AcceptTerms { get; init; }
}

/// <summary>
/// Solicitud de cambio de contraseña
/// </summary>
public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Contraseña actual es requerida")]
    public required string CurrentPassword { get; init; }

    [Required(ErrorMessage = "Nueva contraseña es requerida")]
    [MinLength(8, ErrorMessage = "Contraseña debe tener al menos 8 caracteres")]
    public required string NewPassword { get; init; }

    [Required(ErrorMessage = "Confirmar contraseña es requerido")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public required string ConfirmPassword { get; init; }
}

/// <summary>
/// Solicitud de reset de contraseña (primer paso)
/// </summary>
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; init; }
}

/// <summary>
/// Solicitud para resetear contraseña con token
/// </summary>
public class ResetPasswordRequest
{
    [Required]
    public required string Token { get; init; }

    [Required(ErrorMessage = "Email es requerido")]
    [EmailAddress]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Nueva contraseña es requerida")]
    [MinLength(8)]
    public required string Password { get; init; }

    [Required]
    [Compare("Password")]
    public required string ConfirmPassword { get; init; }
}

/// <summary>
/// Solicitud de refresh token
/// </summary>
public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token es requerido")]
    public required string RefreshToken { get; init; }
}

/// <summary>
/// Solicitud de verificación de email
/// </summary>
public class VerifyEmailRequest
{
    [Required]
    public required string Token { get; init; }

    [Required(ErrorMessage = "Email es requerido")]
    [EmailAddress]
    public required string Email { get; init; }
}

/// <summary>
/// Solicitud de OAuth callback
/// </summary>
public class OAuthCallbackRequest
{
    [Required(ErrorMessage = "Code es requerido")]
    public required string Code { get; init; }

    [Required(ErrorMessage = "State es requerido")]
    public required string State { get; init; }

    public string? RedirectUri { get; init; }
}

/// <summary>
/// Solicitud de login con OAuth token
/// </summary>
public class OAuthTokenRequest
{
    [Required(ErrorMessage = "ID Token es requerido")]
    public required string IdToken { get; init; }

    [Required(ErrorMessage = "Provider es requerido")]
    [RegularExpression("^(google|github)$", ErrorMessage = "Provider debe ser 'google' o 'github'")]
    public required string Provider { get; init; }
}
