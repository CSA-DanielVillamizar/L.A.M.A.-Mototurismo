using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;

namespace Lama.Application.Abstractions;

/// <summary>
/// Interfaz abstracta para el DbContext de la aplicación
/// Permite que la capa Application sea independiente de Infrastructure
/// </summary>
public interface ILamaDbContext
{
    /// <summary>
    /// DbSet para la entidad MemberStatusType (tipos de estado de miembros)
    /// </summary>
    DbSet<MemberStatusType> MemberStatusTypes { get; }

    /// <summary>
    /// DbSet para la entidad Member (miembros de LAMA)
    /// </summary>
    DbSet<Member> Members { get; }

    /// <summary>
    /// DbSet para la entidad Vehicle (vehículos)
    /// </summary>
    DbSet<Vehicle> Vehicles { get; }

    /// <summary>
    /// DbSet para la entidad Event (eventos)
    /// </summary>
    DbSet<Event> Events { get; }

    /// <summary>
    /// DbSet para la entidad Attendance (asistencias)
    /// </summary>
    DbSet<Attendance> Attendance { get; }

    /// <summary>
    /// DbSet para la entidad IdentityUser (usuarios de Entra ID)
    /// </summary>
    DbSet<IdentityUser> IdentityUsers { get; }

    /// <summary>
    /// DbSet para la entidad UserRole (roles asignados a usuarios)
    /// </summary>
    DbSet<UserRole> UserRoles { get; }

    /// <summary>
    /// DbSet para la entidad UserScope (scopes asignados a usuarios)
    /// </summary>
    DbSet<UserScope> UserScopes { get; }

    /// <summary>
    /// DbSet para la entidad Evidence (evidencias fotográficas)
    /// </summary>
    DbSet<Evidence> Evidences { get; }

    /// <summary>
    /// Guarda los cambios en la base de datos de forma asincrónica
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de entidades afectadas</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
