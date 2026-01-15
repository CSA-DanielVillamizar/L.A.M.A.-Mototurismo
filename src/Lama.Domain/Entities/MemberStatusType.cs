namespace Lama.Domain.Entities;

/// <summary>
/// Tipos de estado de miembros disponibles en el sistema.
/// Los valores provienen de la lista desplegable del Excel (Resumen!E4:E36)
/// </summary>
public class MemberStatusType
{
    /// <summary>
    /// Identificador único del tipo de estado
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Nombre del estado (ej: CHAPTER PRESIDENT, PROSPECT, etc.)
    /// Corresponde exactamente a los valores en el Excel
    /// </summary>
    public string StatusName { get; set; } = null!;

    /// <summary>
    /// Categoría del estado para agrupamiento:
    /// - CHAPTER: Nivel capítulo
    /// - CHAPTER_OFFICER: Oficial de capítulo
    /// - REGIONAL_OFFICER: Oficial regional
    /// - NATIONAL_OFFICER: Oficial nacional
    /// - CONTINENTAL_OFFICER: Oficial continental
    /// - INTERNATIONAL_OFFICER: Oficial internacional
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// Orden de visualización en dropdowns y listas
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Indica si el estado está activo (disponible para seleccionar)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    /// <summary>
    /// Miembros que tienen este estado
    /// </summary>
    public ICollection<Member> Members { get; set; } = new List<Member>();
}
