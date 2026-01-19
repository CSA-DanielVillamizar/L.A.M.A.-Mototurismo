using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;
using Lama.Application.Repositories;

namespace Lama.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para Event
/// </summary>
public class EventRepository(LamaDbContext context) : IEventRepository
{
    private readonly LamaDbContext _context = context;

    public async Task<Event?> GetByIdAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Attendances)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByChapterAsync(int chapterId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .AsNoTracking()
            .Where(e => e.ChapterId == chapterId)
            .OrderBy(e => e.EventStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .AsNoTracking()
            .OrderBy(e => e.EventStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
    }

    public Task UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(@event);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await GetByIdAsync(eventId, cancellationToken);
        if (@event != null)
        {
            _context.Events.Remove(@event);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
