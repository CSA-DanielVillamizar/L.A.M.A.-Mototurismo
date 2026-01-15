namespace Lama.Application.DTOs;

/// <summary>
/// DTO para tipo de estado de miembro (solo lectura)
/// Usado en dropdowns y selecciones del formulario COR
/// </summary>
public class MemberStatusTypeDto
{
    /// <summary>
    /// Identificador del tipo de estado
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Nombre del estado para mostrar en dropdown
    /// Ej: "CHAPTER PRESIDENT", "PROSPECT", etc.
    /// </summary>
    public string StatusName { get; set; } = null!;

    /// <summary>
    /// Categoría del estado (usado para agrupar en dropdowns)
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Extensión para mapping desde entidad a DTO
/// </summary>
public static class MemberStatusTypeDtoExtensions
{
    public static MemberStatusTypeDto ToDto(this Domain.Entities.MemberStatusType statusType)
    {
        return new MemberStatusTypeDto
        {
            StatusId = statusType.StatusId,
            StatusName = statusType.StatusName,
            Category = statusType.Category,
            DisplayOrder = statusType.DisplayOrder
        };
    }

    public static IEnumerable<MemberStatusTypeDto> ToDto(this IEnumerable<Domain.Entities.MemberStatusType> statusTypes)
    {
        return statusTypes.Select(s => s.ToDto());
    }
}
