using Lama.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para la entidad AuditLog.
/// Define tabla, índices y restricciones.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        // Clave primaria
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        // Propiedades requeridas
        builder.Property(a => a.TenantId).IsRequired();
        builder.Property(a => a.ActorExternalSubjectId).IsRequired().HasMaxLength(255);
        builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Action).IsRequired();
        builder.Property(a => a.EntityType).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Propiedades opcionales
        builder.Property(a => a.ActorMemberId).IsRequired(false);
        builder.Property(a => a.BeforeJson).IsRequired(false).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.AfterJson).IsRequired(false).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.Notes).IsRequired(false).HasMaxLength(1000);
        builder.Property(a => a.CorrelationId).IsRequired(false).HasMaxLength(100);
        builder.Property(a => a.IpAddress).IsRequired(false).HasMaxLength(45); // IPv6 max 45 chars
        builder.Property(a => a.UserAgent).IsRequired(false).HasMaxLength(500);

        // Índices para búsquedas eficientes
        // Índice compuesto: buscar auditorías por miembro dentro de un tenant
        builder.HasIndex(a => new { a.TenantId, a.ActorMemberId, a.CreatedAt })
            .HasName("IX_AuditLogs_TenantMemberDate");

        // Índice compuesto: buscar auditorías de una entidad específica
        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId, a.CreatedAt })
            .HasName("IX_AuditLogs_TenantEntityDate");

        // Índice para búsquedas por CorrelationId (rastreo distribuido)
        builder.HasIndex(a => a.CorrelationId)
            .HasName("IX_AuditLogs_CorrelationId");

        // Índice para búsquedas por tipo de acción (análisis agregado)
        builder.HasIndex(a => new { a.TenantId, a.Action, a.CreatedAt })
            .HasName("IX_AuditLogs_TenantActionDate");

        // Índice para búsquedas por fecha (limpieza de datos antiguos, resúmenes)
        builder.HasIndex(a => new { a.TenantId, a.CreatedAt })
            .HasName("IX_AuditLogs_TenantDate");

        // Índice para detectar intentos de acceso no autorizado
        builder.HasIndex(a => new { a.TenantId, a.IpAddress, a.CreatedAt })
            .HasName("IX_AuditLogs_TenantIpDate");
    }
}
