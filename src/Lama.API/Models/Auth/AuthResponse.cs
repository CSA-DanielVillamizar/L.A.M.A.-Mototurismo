using System.Text.Json.Serialization;

namespace Lama.API.Models.Auth;

/// <summary>
/// Respuesta exitosa de autenticación
/// </summary>
public class AuthResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; } = 3600; // 1 hora
    public required UserDto User { get; init; }
}

/// <summary>
/// Información de usuario en respuesta de auth
/// </summary>
public class UserDto
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Avatar { get; init; }
    public bool EmailVerified { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<string> Roles { get; init; } = new();
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Respuesta de refresh token
/// </summary>
public class RefreshTokenResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public int ExpiresIn { get; init; } = 3600;
}

/// <summary>
/// Respuesta de operación de seguridad (logout, email verify, etc)
/// </summary>
public class MessageResponse
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("success")]
    public bool Success { get; init; } = true;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Respuesta de verificación de email
/// </summary>
public class VerifyEmailResponse
{
    public required string Message { get; init; }
    public bool EmailVerified { get; init; }
}

/// <summary>
/// Respuesta de perfil de usuario
/// </summary>
public class UserProfileResponse
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Phone { get; init; }
    public string? Avatar { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool EmailVerified { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public List<string> Roles { get; init; } = new();
    public Dictionary<string, string> LinkedProviders { get; init; } = new();
}

/// <summary>
/// Respuesta de OAuth exchange
/// </summary>
public class OAuthExchangeResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public bool IsNewUser { get; init; }
    public required UserDto User { get; init; }
}
