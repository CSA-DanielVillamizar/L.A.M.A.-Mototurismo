using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Configuration
/// </summary>
public class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> builder)
    {
        builder.ToTable("Configuration");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Key)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Value)
            .IsRequired();

        builder.Property(c => c.Description);

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(c => c.Key)
            .IsUnique();
    }
}
