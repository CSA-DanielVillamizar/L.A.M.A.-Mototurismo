using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Lama.API.Routing;

/// <summary>
/// Transformador que convierte tokens de ruta (controller/action) a kebab-case
/// para exponer URLs consistentes y amigables para la web moderna.
/// </summary>
public sealed class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    private static readonly Regex Pattern = new("([a-z0-9])([A-Z])", RegexOptions.Compiled);

    /// <summary>
    /// Convierte el token saliente a kebab-case (ej. MemberStatusTypes -> member-status-types).
    /// </summary>
    /// <param name="value">Valor del token de ruta.</param>
    /// <returns>Token convertido a kebab-case o null si el valor está vacío.</returns>
    public string? TransformOutbound(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var input = value.ToString();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var kebab = Pattern.Replace(input, "$1-$2");
        return kebab.ToLowerInvariant();
    }
}
