using System.Security.Claims;

namespace Lama.API.Utilities;

/// <summary>
/// Utilidad para extraer y validar información de los claims de JWT tokens.
/// Proporciona métodos para obtener IDs externos (sub claim) y otros datos del usuario.
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// Nombre del claim para el identificador externo del usuario (corresponde a "sub" en JWT estándar).
    /// En Azure Entra ID, este es el ObjectId del usuario.
    /// </summary>
    private const string ExternalSubjectIdClaimType = "sub";

    /// <summary>
    /// Nombre del claim para el identificador del tenant (corresponde a "tid" en Azure Entra ID).
    /// </summary>
    private const string TenantIdClaimType = "tid";

    /// <summary>
    /// Obtiene el identificador externo del usuario desde los claims.
    /// Este identificador viene del claim "sub" que contendrá el ObjectId del usuario en Entra ID.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario autenticado</param>
    /// <returns>El identificador externo del usuario</returns>
    /// <exception cref="InvalidOperationException">Si no se encuentra el claim "sub"</exception>
    public static string GetExternalSubjectId(ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var subClaim = user.FindFirst(ExternalSubjectIdClaimType);
        if (subClaim == null || string.IsNullOrWhiteSpace(subClaim.Value))
            throw new InvalidOperationException($"El usuario no tiene el claim '{ExternalSubjectIdClaimType}' requerido.");

        return subClaim.Value;
    }

    /// <summary>
    /// Intenta obtener el identificador externo del usuario desde los claims.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario</param>
    /// <param name="externalSubjectId">El identificador externo si se encuentra; null en caso contrario</param>
    /// <returns>True si se encontró el claim; false en caso contrario</returns>
    public static bool TryGetExternalSubjectId(ClaimsPrincipal user, out string? externalSubjectId)
    {
        externalSubjectId = null;

        if (user == null)
            return false;

        var subClaim = user.FindFirst(ExternalSubjectIdClaimType);
        if (subClaim == null || string.IsNullOrWhiteSpace(subClaim.Value))
            return false;

        externalSubjectId = subClaim.Value;
        return true;
    }

    /// <summary>
    /// Obtiene el identificador del tenant desde los claims.
    /// Este identificador viene del claim "tid" en Azure Entra ID.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario</param>
    /// <returns>El identificador del tenant como GUID</returns>
    /// <exception cref="InvalidOperationException">Si no se encuentra el claim "tid" o no es un GUID válido</exception>
    public static Guid GetTenantIdFromClaims(ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var tidClaim = user.FindFirst(TenantIdClaimType);
        if (tidClaim == null || string.IsNullOrWhiteSpace(tidClaim.Value))
            throw new InvalidOperationException($"El usuario no tiene el claim '{TenantIdClaimType}' requerido.");

        if (!Guid.TryParse(tidClaim.Value, out var tenantId))
            throw new InvalidOperationException($"El valor del claim '{TenantIdClaimType}' no es un GUID válido: {tidClaim.Value}");

        return tenantId;
    }

    /// <summary>
    /// Intenta obtener el identificador del tenant desde los claims.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario</param>
    /// <param name="tenantId">El identificador del tenant si se encuentra; Guid.Empty en caso contrario</param>
    /// <returns>True si se encontró y se pudo parsear el claim; false en caso contrario</returns>
    public static bool TryGetTenantId(ClaimsPrincipal user, out Guid tenantId)
    {
        tenantId = Guid.Empty;

        if (user == null)
            return false;

        var tidClaim = user.FindFirst(TenantIdClaimType);
        if (tidClaim == null || string.IsNullOrWhiteSpace(tidClaim.Value))
            return false;

        return Guid.TryParse(tidClaim.Value, out tenantId);
    }

    /// <summary>
    /// Extrae todos los claims del usuario como un diccionario.
    /// Útil para debugging y logging de claims.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario</param>
    /// <returns>Diccionario con tipo de claim como clave y valor como valor</returns>
    public static Dictionary<string, string> ExtractAllClaims(ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return user.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ", g.Select(c => c.Value)));
    }

    /// <summary>
    /// Obtiene un claim específico por su tipo.
    /// </summary>
    /// <param name="user">ClaimsPrincipal que contiene los claims del usuario</param>
    /// <param name="claimType">Tipo del claim a obtener</param>
    /// <returns>El valor del claim, o null si no existe</returns>
    public static string? GetClaimValue(ClaimsPrincipal user, string claimType)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(claimType))
            throw new ArgumentException("ClaimType no puede estar vacío", nameof(claimType));

        return user.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Valida que un usuario autenticado tenga los claims requeridos.
    /// </summary>
    /// <param name="user">ClaimsPrincipal a validar</param>
    /// <returns>True si el usuario tiene los claims "sub" y "tid"; false en caso contrario</returns>
    public static bool ValidateRequiredClaims(ClaimsPrincipal user)
    {
        if (user == null)
            return false;

        return TryGetExternalSubjectId(user, out _) && TryGetTenantId(user, out _);
    }
}
