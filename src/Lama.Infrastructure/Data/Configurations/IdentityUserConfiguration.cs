using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración Fluent API para la entidad IdentityUser
/// </summary>
public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        // Tabla
        builder.ToTable("IdentityUsers");

        // Primary Key
        builder.HasKey(iu => iu.Id);

        // Propiedades
        builder.Property(iu => iu.Id)
            .ValueGeneratedOnAdd();

        builder.Property(iu => iu.TenantId)
            .IsRequired();

        builder.Property(iu => iu.ExternalSubjectId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(iu => iu.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(iu => iu.DisplayName)
            .HasMaxLength(500);

        builder.Property(iu => iu.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(iu => iu.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(iu => iu.LastLoginAt);

        builder.Property(iu => iu.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(iu => iu.Member)
            .WithMany()
            .HasForeignKey(iu => iu.MemberId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(iu => new { iu.TenantId, iu.ExternalSubjectId })
            .IsUnique()
            .HasDatabaseName("UX_IdentityUsers_TenantId_ExternalSubjectId");

        builder.HasIndex(iu => new { iu.TenantId, iu.Email })
            .HasDatabaseName("IX_IdentityUsers_TenantId_Email");

        builder.HasIndex(iu => iu.TenantId)
            .HasDatabaseName("IX_IdentityUsers_TenantId");

        builder.HasIndex(iu => iu.CreatedAt)
            .HasDatabaseName("IX_IdentityUsers_CreatedAt");
    }
}
