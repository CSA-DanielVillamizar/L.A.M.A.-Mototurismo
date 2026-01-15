namespace Lama.Domain.Enums;

/// <summary>
/// Clasificación de visitante en un evento (bonus de puntos)
/// </summary>
public enum VisitorClass
{
    /// <summary>Local (mismo país)</summary>
    Local,
    
    /// <summary>Visitante A (otro país, mismo continente)</summary>
    VisitorA,
    
    /// <summary>Visitante B (otro continente)</summary>
    VisitorB
}
