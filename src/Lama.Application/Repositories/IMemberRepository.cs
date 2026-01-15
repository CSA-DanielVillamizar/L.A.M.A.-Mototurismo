using Lama.Domain.Entities;

namespace Lama.Application.Repositories;

/// <summary>
/// Interfaz repositorio para la entidad Member
/// </summary>
public interface IMemberRepository
{
    /// <summary>Obtiene un miembro por su ID</summary>
    Task<Member?> GetByIdAsync(int memberId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un miembro por el número de orden</summary>
    Task<Member?> GetByOrderAsync(int order, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los miembros activos de un capítulo</summary>
    Task<IEnumerable<Member>> GetByChapterAsync(int chapterId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los miembros</summary>
    Task<IEnumerable<Member>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo miembro</summary>
    Task AddAsync(Member member, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un miembro</summary>
    Task UpdateAsync(Member member, CancellationToken cancellationToken = default);

    /// <summary>Elimina un miembro</summary>
    Task DeleteAsync(int memberId, CancellationToken cancellationToken = default);

    /// <summary>Guarda cambios en la base de datos</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
