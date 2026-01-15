using Lama.Domain.Entities;

namespace Lama.Application.Repositories;

/// <summary>
/// Interfaz repositorio para la entidad Attendance
/// </summary>
public interface IAttendanceRepository
{
    /// <summary>Obtiene un registro de asistencia por su ID</summary>
    Task<Attendance?> GetByIdAsync(int attendanceId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene asistencia de un miembro a un evento</summary>
    Task<Attendance?> GetByMemberEventAsync(int memberId, int eventId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todas las asistencias a un evento</summary>
    Task<IEnumerable<Attendance>> GetByEventAsync(int eventId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todas las asistencias de un miembro</summary>
    Task<IEnumerable<Attendance>> GetByMemberAsync(int memberId, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todas las asistencias pendientes</summary>
    Task<IEnumerable<Attendance>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo registro de asistencia</summary>
    Task AddAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un registro de asistencia</summary>
    Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>Elimina un registro de asistencia</summary>
    Task DeleteAsync(int attendanceId, CancellationToken cancellationToken = default);

    /// <summary>Guarda cambios en la base de datos</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
