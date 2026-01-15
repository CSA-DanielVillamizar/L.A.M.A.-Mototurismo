using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Infrastructure.Data.Configurations;
using Lama.Application.Abstractions;

namespace Lama.Infrastructure.Data;

/// <summary>
/// DbContext de EF Core para la plataforma LAMA Mototurismo
/// Implementa la interfaz ILamaDbContext de la capa Application para respetar Clean Architecture
/// </summary>
public class LamaDbContext : DbContext, ILamaDbContext
{
    public LamaDbContext(DbContextOptions<LamaDbContext> options)
        : base(options)
    {
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
    }
}
