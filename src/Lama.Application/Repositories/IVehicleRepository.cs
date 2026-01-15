using Lama.Domain.Entities;

namespace Lama.Application.Repositories;

/// <summary>
/// Interfaz repositorio para la entidad Vehicle
/// </summary>
public interface IVehicleRepository
{
    /// <summary>Obtiene un vehículo por su ID</summary>
    Task<Vehicle?> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un vehículo por placa de licencia</summary>
    Task<Vehicle?> GetByLicPlateAsync(string licPlate, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los vehículos activos de un miembro</summary>
    Task<IEnumerable<Vehicle>> GetActiveByMemberAsync(int memberId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los vehículos de un miembro</summary>
    Task<IEnumerable<Vehicle>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo vehículo</summary>
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un vehículo</summary>
    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>Elimina un vehículo</summary>
    Task DeleteAsync(int vehicleId, CancellationToken cancellationToken = default);

    /// <summary>Guarda cambios en la base de datos</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
