using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;
using Lama.Application.Repositories;
using Lama.Application.DTOs;

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
            .AsNoTracking()
            .Include(a => a.Event)
            .Include(a => a.Member)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.Id == attendanceId, cancellationToken);
    }

    public async Task<Attendance?> GetByMemberEventAsync(int memberId, int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.MemberId == memberId && a.EventId == eventId, cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetByEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .AsNoTracking()
            .Where(a => a.EventId == eventId)
            .Include(a => a.Member)
            .Include(a => a.Vehicle)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .AsNoTracking()
            .Where(a => a.MemberId == memberId)
            .Include(a => a.Event)
            .OrderByDescending(a => a.Event!.EventStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Attendance>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Attendance
            .AsNoTracking()
            .Where(a => a.Status == "PENDING")
            .Include(a => a.Member)
            .Include(a => a.Event)
            .Include(a => a.Vehicle)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResultDto<AdminQueueItemDto>> GetAdminQueueAsync(
        Guid tenantId,
        int? eventId = null,
        string? status = null,
        string? searchQuery = null,
        int page = 1,
        int pageSize = 20,
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        // Validar y normalizar parámetros
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 10, 100);
        
        // Query base - aplicar todos los filtros primero
        IQueryable<Attendance> baseQuery = _context.Attendance
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId);

        // Aplicar filtros
        if (eventId.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.EventId == eventId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.ToUpperInvariant();
            baseQuery = baseQuery.Where(a => a.Status == normalizedStatus);
        }

        // Búsqueda full-text (case-insensitive)
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.ToLowerInvariant();
            baseQuery = baseQuery.Where(a =>
                a.Member!.CompleteNames.ToLower().Contains(q) ||
                a.Vehicle!.LicPlate.ToLower().Contains(q) ||
                a.Event!.NameOfTheEvent.ToLower().Contains(q));
        }

        // Contar total
        var total = await baseQuery.CountAsync(cancellationToken);

        // Aplicar ordenamiento
        var orderedQuery = ApplyOrdering(baseQuery, sort);

        // Paginar e incluir datos relacionados
        var items = await orderedQuery
            .Include(a => a.Event)
            .Include(a => a.Member)
            .Include(a => a.Vehicle)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AdminQueueItemDto
            {
                AttendanceId = a.Id,
                TenantId = a.TenantId,
                EventId = a.EventId,
                EventName = a.Event!.NameOfTheEvent,
                EventDate = a.Event!.EventStartDate,
                MemberId = a.MemberId,
                MemberName = a.Member!.CompleteNames,
                MemberEmail = string.Empty,
                ChapterId = a.Member!.ChapterId,
                VehicleId = a.VehicleId,
                VehicleLicPlate = a.Vehicle!.LicPlate,
                VehicleMotorcycleData = a.Vehicle!.MotorcycleData,
                Status = a.Status,
                PointsPerEvent = a.PointsPerEvent,
                PointsPerDistance = a.PointsPerDistance,
                PointsAwardedPerMember = a.PointsAwardedPerMember,
                VisitorClass = a.VisitorClass,
                ConfirmedAt = a.ConfirmedAt,
                ConfirmedBy = a.ConfirmedBy,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AdminQueueItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    /// <summary>
    /// Aplica ordenamiento a la query según el parámetro sort
    /// </summary>
    private static IQueryable<Attendance> ApplyOrdering(IQueryable<Attendance> query, string? sort)
    {
        return sort?.ToLowerInvariant() switch
        {
            "createdat_asc" => query.OrderBy(a => a.CreatedAt),
            "createdat_desc" => query.OrderByDescending(a => a.CreatedAt),
            "eventdate_asc" => query.OrderBy(a => a.Event!.EventStartDate),
            "eventdate_desc" => query.OrderByDescending(a => a.Event!.EventStartDate),
            "member_asc" => query.OrderBy(a => a.Member!.CompleteNames),
            "points_desc" => query.OrderByDescending(a => a.PointsAwardedPerMember),
            // Default: PENDING primero, luego por fecha de creación descendente
            _ => query
                .OrderBy(a => a.Status != "PENDING") // PENDING = false (0), otros = true (1)
                .ThenByDescending(a => a.CreatedAt)
        };
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
