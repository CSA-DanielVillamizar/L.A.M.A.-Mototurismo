using Lama.Domain.Entities;
using Lama.Application.DTOs;

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

    /// <summary>Obtiene la cola de admin con filtros, búsqueda y paginación</summary>
    /// <param name="tenantId">ID del tenant para aislamiento multi-tenant</param>
    /// <param name="eventId">Filtro opcional: ID del evento</param>
    /// <param name="status">Filtro opcional: Estado (PENDING, CONFIRMED, REJECTED)</param>
    /// <param name="searchQuery">Búsqueda opcional en nombre de miembro, placa de vehículo o evento</param>
    /// <param name="page">Número de página (1-indexed, mínimo 1)</param>
    /// <param name="pageSize">Items por página (10-100, default 20)</param>
    /// <param name="sort">Ordenamiento: createdAt_desc|createdAt_asc|eventDate_desc|eventDate_asc|member_asc|points_desc</param>
    Task<PagedResultDto<AdminQueueItemDto>> GetAdminQueueAsync(
        Guid tenantId,
        int? eventId = null,
        string? status = null,
        string? searchQuery = null,
        int page = 1,
        int pageSize = 20,
        string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo registro de asistencia</summary>
    Task AddAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un registro de asistencia</summary>
    Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>Elimina un registro de asistencia</summary>
    Task DeleteAsync(int attendanceId, CancellationToken cancellationToken = default);

    /// <summary>Guarda cambios en la base de datos</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
