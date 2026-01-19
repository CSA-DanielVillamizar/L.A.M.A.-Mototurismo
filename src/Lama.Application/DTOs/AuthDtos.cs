namespace Lama.Application.DTOs;

/// <summary>
/// Respuesta de sesión de autenticación
/// </summary>
public class AuthSessionResponse
{
    /// <summary>Access token de la aplicación (JWT interno, corta vida)</summary>
    public required string AccessToken { get; set; }

    /// <summary>Tipo de token (siempre "Bearer")</summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>Segundos hasta la expiración del access token</summary>
    public int ExpiresIn { get; set; }

    /// <summary>Información del usuario autenticado</summary>
    public required UserInfo User { get; set; }
}

/// <summary>
/// Información del usuario autenticado
/// </summary>
public class UserInfo
{
    /// <summary>ID del usuario de identidad</summary>
    public int Id { get; set; }

    /// <summary>Email del usuario</summary>
    public required string Email { get; set; }

    /// <summary>Nombre para mostrar</summary>
    public string? DisplayName { get; set; }

    /// <summary>ID del miembro de LAMA asociado (si existe)</summary>
    public int? MemberId { get; set; }

    /// <summary>Nombre completo del miembro (si existe)</summary>
    public string? MemberName { get; set; }

    /// <summary>Capítulo del miembro (si existe)</summary>
    public string? ChapterName { get; set; }

    /// <summary>Roles del usuario</summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>Scopes del usuario</summary>
    public List<string> Scopes { get; set; } = new();

    /// <summary>ID del tenant</summary>
    public Guid TenantId { get; set; }
}

/// <summary>
/// Request para intercambiar token de Entra ID
/// </summary>
public class ExchangeTokenRequest
{
    /// <summary>ID Token o Access Token de Entra ID</summary>
    public required string IdToken { get; set; }
}

/// <summary>
/// Response de logout
/// </summary>
public class LogoutResponse
{
    /// <summary>Mensaje de confirmación</summary>
    public required string Message { get; set; }
}
