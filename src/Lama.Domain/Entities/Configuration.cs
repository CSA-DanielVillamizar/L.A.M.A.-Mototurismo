namespace Lama.Domain.Entities;

/// <summary>
/// Configuración global de la plataforma
/// </summary>
public class Configuration
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>Clave de configuración (ej. DistanceThreshold_1Point_OneWayMiles)</summary>
    public required string Key { get; set; }

    /// <summary>Valor de la configuración</summary>
    public required string Value { get; set; }

    /// <summary>Descripción de la configuración</summary>
    public string? Description { get; set; }

    /// <summary>Última fecha de actualización</summary>
    public DateTime UpdatedAt { get; set; }
}
