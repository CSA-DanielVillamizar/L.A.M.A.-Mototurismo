using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Capítulo de LAMA (grupo regional)
/// </summary>
public class Chapter
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>Nombre del capítulo</summary>
    public required string Name { get; set; }

    /// <summary>País donde se localiza el capítulo</summary>
    public required string Country { get; set; }

    /// <summary>Fecha de creación</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Indica si el capítulo está activo</summary>
    public bool IsActive { get; set; }

    /// <summary>Relación: Miembros del capítulo</summary>
    public ICollection<Member> Members { get; set; } = [];

    /// <summary>Relación: Eventos del capítulo</summary>
    public ICollection<Event> Events { get; set; } = [];
}
