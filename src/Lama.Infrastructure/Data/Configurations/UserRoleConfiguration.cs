using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración Fluent API para UserRole entity
/// Define table name, properties, constraints, e índices para optimizar queries
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Nombre de tabla
        builder.ToTable("UserRoles");

        // Clave primaria
        builder.HasKey(ur => ur.Id);

        // Propiedades
        builder.Property(ur => ur.TenantId)
            .IsRequired();

        builder.Property(ur => ur.ExternalSubjectId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ur => ur.Role)
            .IsRequired();

        builder.Property(ur => ur.AssignedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(ur => ur.IsActive)
            .HasDefaultValue(true);

        builder.Property(ur => ur.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(ur => ur.Reason)
            .HasMaxLength(500);

        builder.Property(ur => ur.AssignedBy)
            .HasMaxLength(255);

        // Índices para optimizar búsquedas
        builder.HasIndex(ur => new { ur.TenantId, ur.ExternalSubjectId })
            .HasDatabaseName("IX_UserRoles_TenantId_ExternalSubjectId");

        builder.HasIndex(ur => new { ur.TenantId, ur.Role })
            .HasDatabaseName("IX_UserRoles_TenantId_Role");

        builder.HasIndex(ur => new { ur.TenantId, ur.ExternalSubjectId, ur.IsActive })
            .HasDatabaseName("IX_UserRoles_TenantId_ExternalSubjectId_IsActive");

        builder.HasIndex(ur => ur.AssignedAt)
            .HasDatabaseName("IX_UserRoles_AssignedAt");

        // Conversión de enum a string para mejor legibilidad en BD
        builder.Property(ur => ur.Role)
            .HasConversion<string>();
    }
}
