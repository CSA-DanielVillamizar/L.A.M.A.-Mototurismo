using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración Fluent API para UserScope entity
/// Define table name, properties, constraints, e índices para optimizar queries de autorización
/// </summary>
public class UserScopeConfiguration : IEntityTypeConfiguration<UserScope>
{
    public void Configure(EntityTypeBuilder<UserScope> builder)
    {
        // Nombre de tabla
        builder.ToTable("UserScopes");

        // Clave primaria
        builder.HasKey(us => us.Id);

        // Propiedades
        builder.Property(us => us.TenantId)
            .IsRequired();

        builder.Property(us => us.ExternalSubjectId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(us => us.ScopeType)
            .IsRequired();

        builder.Property(us => us.ScopeId)
            .HasMaxLength(255);

        builder.Property(us => us.AssignedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(us => us.IsActive)
            .HasDefaultValue(true);

        builder.Property(us => us.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(us => us.Reason)
            .HasMaxLength(500);

        builder.Property(us => us.AssignedBy)
            .HasMaxLength(255);

        // Índices para optimizar búsquedas frecuentes en autorización
        builder.HasIndex(us => new { us.TenantId, us.ExternalSubjectId })
            .HasDatabaseName("IX_UserScopes_TenantId_ExternalSubjectId");

        builder.HasIndex(us => new { us.TenantId, us.ExternalSubjectId, us.ScopeType })
            .HasDatabaseName("IX_UserScopes_TenantId_ExternalSubjectId_ScopeType");

        builder.HasIndex(us => new { us.TenantId, us.ExternalSubjectId, us.IsActive })
            .HasDatabaseName("IX_UserScopes_TenantId_ExternalSubjectId_IsActive");

        builder.HasIndex(us => new { us.TenantId, us.ScopeType, us.ScopeId })
            .HasDatabaseName("IX_UserScopes_TenantId_ScopeType_ScopeId");

        builder.HasIndex(us => us.AssignedAt)
            .HasDatabaseName("IX_UserScopes_AssignedAt");

        // Conversión de enum a string
        builder.Property(us => us.ScopeType)
            .HasConversion<string>();
    }
}
