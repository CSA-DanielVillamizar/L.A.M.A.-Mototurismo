namespace Lama.API.Configuration;

/// <summary>
/// Configuración de Microsoft Entra External ID (Azure B2C)
/// Se carga desde appsettings.json en la sección "AzureAd"
/// </summary>
public class AzureAdOptions
{
    /// <summary>Autoridad de token (instance + tenant)</summary>
    /// <example>https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin</example>
    public string? Authority { get; set; }

    /// <summary>ID de cliente de la aplicación Azure B2C</summary>
    public string? ClientId { get; set; }

    /// <summary>Audience esperado en el JWT (generalmente igual a ClientId)</summary>
    public string? Audience { get; set; }

    /// <summary>Secret de la aplicación (NO SE USA EN BEARER AUTH - solo en flujos de servidor)</summary>
    public string? ClientSecret { get; set; }

    /// <summary>URL de redirección post-login (para flujos con UI)</summary>
    public string? RedirectUri { get; set; }

    /// <summary>Scopes requeridos (por defecto: openid profile email)</summary>
    public string Scopes { get; set; } = "openid profile email";

    /// <summary>Indica si se requiere autenticación para endpoints públicos</summary>
    public bool RequireAuthentication { get; set; } = true;
}
