using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data;
using Lama.Application.Repositories;

namespace Lama.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para Vehicle
/// </summary>
public class VehicleRepository(LamaDbContext context) : IVehicleRepository
{
    private readonly LamaDbContext _context = context;

    public async Task<Vehicle?> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Member)
            .Include(v => v.Attendances)
            .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);
    }

    public async Task<Vehicle?> GetByLicPlateAsync(string licPlate, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.LicPlate == licPlate, cancellationToken);
    }

    public async Task<IEnumerable<Vehicle>> GetActiveByMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.MemberId == memberId && v.IsActiveForChampionship)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Vehicle>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
    }

    public Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
