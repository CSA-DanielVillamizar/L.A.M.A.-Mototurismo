using Lama.Domain.Entities;

namespace Lama.Application.Repositories;

/// <summary>
/// Interfaz repositorio para la entidad Event
/// </summary>
public interface IEventRepository
{
    /// <summary>Obtiene un evento por su ID</summary>
    Task<Event?> GetByIdAsync(int eventId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los eventos de un capítulo</summary>
    Task<IEnumerable<Event>> GetByChapterAsync(int chapterId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los eventos</summary>
    Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo evento</summary>
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un evento</summary>
    Task UpdateAsync(Event @event, CancellationToken cancellationToken = default);

    /// <summary>Elimina un evento</summary>
    Task DeleteAsync(int eventId, CancellationToken cancellationToken = default);

    /// <summary>Guarda cambios en la base de datos</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
