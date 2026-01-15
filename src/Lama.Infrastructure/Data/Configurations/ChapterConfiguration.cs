using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Chapter
/// </summary>
public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("Chapters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Chapter)
            .HasForeignKey(m => m.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Events)
            .WithOne(e => e.Chapter)
            .HasForeignKey(e => e.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
