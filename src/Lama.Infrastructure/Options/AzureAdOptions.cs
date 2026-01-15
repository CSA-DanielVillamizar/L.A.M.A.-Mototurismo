namespace Lama.Infrastructure.Options;

/// <summary>
/// Configuración de autenticación de Microsoft Entra External ID (Azure B2C)
/// Se carga desde appsettings.json -> AzureAd
/// </summary>
public class AzureAdOptions
{
    /// <summary>
    /// URL de autoridad de Azure B2C
    /// Ejemplo: https://[tenant].b2clogin.com/[tenant].onmicrosoft.com/B2C_1_signin
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Client ID de la aplicación registrada en Azure B2C
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Audience esperado en el JWT (generalmente = ClientId)
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Client Secret (no necesario para Bearer auth, pero puede usarse en flows de servidor)
    /// NUNCA debe estar en appsettings de RELEASE - usar User Secrets
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Redirect URI para flujos de autenticación interactiva (SPA, etc)
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Scopes requeridos (default: openid profile email)
    /// </summary>
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };

    /// <summary>
    /// Si la autenticación es requerida globalmente (puede desactivarse para testing)
    /// </summary>
    public bool RequireAuthentication { get; set; } = true;
}
