using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data.Configurations;
using Lama.Application.Abstractions;
using Lama.Infrastructure.Services;

namespace Lama.Infrastructure.Data;

/// <summary>
/// DbContext de EF Core para la plataforma LAMA Mototurismo
/// Implementa la interfaz ILamaDbContext de la capa Application para respetar Clean Architecture
/// Incluye query filters automáticos para multi-tenancy
/// </summary>
public class LamaDbContext : DbContext, ILamaDbContext
{
    private readonly ITenantProvider? _tenantProvider;

    // Constructor para EF Core migrations (sin tenantProvider)
    public LamaDbContext(DbContextOptions<LamaDbContext> options)
        : base(options)
    {
        _tenantProvider = null;
    }

    // Constructor con TenantProvider para aplicación en runtime
    public LamaDbContext(DbContextOptions<LamaDbContext> options, ITenantProvider? tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    /// <summary>Tabla de Capítulos</summary>
    public DbSet<Chapter> Chapters { get; set; } = null!;

    /// <summary>Tabla de Miembros</summary>
    public DbSet<Member> Members { get; set; } = null!;

    /// <summary>Tabla de Tipos de Estado de Miembros (dropdown)</summary>
    public DbSet<MemberStatusType> MemberStatusTypes { get; set; } = null!;

    /// <summary>Tabla de Vehículos</summary>
    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    /// <summary>Tabla de Eventos</summary>
    public DbSet<Event> Events { get; set; } = null!;

    /// <summary>Tabla de Asistencias</summary>
    public DbSet<Attendance> Attendance { get; set; } = null!;

    /// <summary>Tabla de Configuración</summary>
    public DbSet<Configuration> Configurations { get; set; } = null!;

    /// <summary>Tabla de Identidades de usuarios (vinculación con Entra ID)</summary>
    public DbSet<IdentityUser> IdentityUsers { get; set; } = null!;

    /// <summary>Tabla de Roles de Usuarios (asignación de roles jerárquicos con auditoría)</summary>
    public DbSet<UserRole> UserRoles { get; set; } = null!;

    /// <summary>Tabla de Scopes de Usuarios (asignación de ámbitos territoriales con auditoría)</summary>
    public DbSet<UserScope> UserScopes { get; set; } = null!;

    /// <summary>Tabla de Evidencias (fotos de piloto + odómetro)</summary>
    public DbSet<Evidence> Evidences { get; set; } = null!;

    /// <summary>Tabla de Snapshots de Ranking (denormalizado para consultas rápidas)</summary>
    public DbSet<RankingSnapshot> RankingSnapshots { get; set; } = null!;

    /// <summary>Tabla de Logs de Auditoría (trazabilidad de acciones auditables)</summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones fluent
        modelBuilder.ApplyConfiguration(new ChapterConfiguration());
        modelBuilder.ApplyConfiguration(new MemberConfiguration());
        modelBuilder.ApplyConfiguration(new MemberStatusTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserScopeConfiguration());
        modelBuilder.ApplyConfiguration(new EvidenceConfiguration());
        modelBuilder.ApplyConfiguration(new RankingSnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        // Query Filters para Multi-Tenancy
        // Estas se aplican automáticamente a todas las queries, sin necesidad de modificar los repositorios
        // Si _tenantProvider es null (testing), no aplicar filtros
        if (_tenantProvider != null)
        {
            // Members: filtrar por TenantId actual
            modelBuilder.Entity<Member>().HasQueryFilter(m => m.TenantId == _tenantProvider.CurrentTenantId);

            // Vehicles: filtrar por TenantId actual
            modelBuilder.Entity<Vehicle>().HasQueryFilter(v => v.TenantId == _tenantProvider.CurrentTenantId);

            // Events: filtrar por TenantId actual
            modelBuilder.Entity<Event>().HasQueryFilter(e => e.TenantId == _tenantProvider.CurrentTenantId);

            // Attendance: filtrar por TenantId actual
            modelBuilder.Entity<Attendance>().HasQueryFilter(a => a.TenantId == _tenantProvider.CurrentTenantId);

            // IdentityUsers: filtrar por TenantId actual
            modelBuilder.Entity<IdentityUser>().HasQueryFilter(iu => iu.TenantId == _tenantProvider.CurrentTenantId);

            // UserRoles: filtrar por TenantId actual
            modelBuilder.Entity<UserRole>().HasQueryFilter(ur => ur.TenantId == _tenantProvider.CurrentTenantId);

            // UserScopes: filtrar por TenantId actual
            modelBuilder.Entity<UserScope>().HasQueryFilter(us => us.TenantId == _tenantProvider.CurrentTenantId);

            // Evidences: filtrar por TenantId actual
            modelBuilder.Entity<Evidence>().HasQueryFilter(e => e.TenantId == _tenantProvider.CurrentTenantId);

            // RankingSnapshots: filtrar por TenantId actual
            modelBuilder.Entity<RankingSnapshot>().HasQueryFilter(rs => rs.TenantId == _tenantProvider.CurrentTenantId);

            // AuditLogs: filtrar por TenantId actual
            modelBuilder.Entity<AuditLog>().HasQueryFilter(al => al.TenantId == _tenantProvider.CurrentTenantId);
        }
    }
}
