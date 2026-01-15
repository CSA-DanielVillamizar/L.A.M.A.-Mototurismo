using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;
using Lama.Application.Repositories;

namespace Lama.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para Attendance
/// </summary>
public class AttendanceRepository(LamaDbContext context) : IAttendanceRepository
{
    private readonly LamaDbContext _context = context;

    public async Task<Attendance?> GetByIdAsync(int attendanceId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .Include(a => a.Event)
            .Include(a => a.Member)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.Id == attendanceId, cancellationToken);
    }

    public async Task<Attendance?> GetByMemberEventAsync(int memberId, int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .FirstOrDefaultAsync(a => a.MemberId == memberId && a.EventId == eventId, cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetByEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .Where(a => a.EventId == eventId)
            .Include(a => a.Member)
            .Include(a => a.Vehicle)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .Where(a => a.MemberId == memberId)
            .Include(a => a.Event)
            .OrderByDescending(a => a.Event!.EventStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .Where(a => a.Status == "PENDING")
            .Include(a => a.Member)
            .Include(a => a.Event)
            .Include(a => a.Vehicle)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Attendance attendance, CancellationToken cancellationToken = default)
    {
        await _context.Attendance.AddAsync(attendance, cancellationToken);
    }

    public Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default)
    {
        _context.Attendance.Update(attendance);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int attendanceId, CancellationToken cancellationToken = default)
    {
        var attendance = await GetByIdAsync(attendanceId, cancellationToken);
        if (attendance != null)
        {
            _context.Attendance.Remove(attendance);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
