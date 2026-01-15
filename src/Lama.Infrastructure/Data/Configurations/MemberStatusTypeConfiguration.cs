using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad MemberStatusType
/// </summary>
public class MemberStatusTypeConfiguration : IEntityTypeConfiguration<MemberStatusType>
{
    public void Configure(EntityTypeBuilder<MemberStatusType> builder)
    {
        builder.ToTable("MemberStatusTypes");

        builder.HasKey(s => s.StatusId);

        builder.Property(s => s.StatusId)
            .HasColumnName("StatusId")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.StatusName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("StatusName");

        builder.Property(s => s.Category)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("Category");

        builder.Property(s => s.DisplayOrder)
            .HasColumnName("DisplayOrder");

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true)
            .HasColumnName("IsActive");

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnName("CreatedAt");

        // Índices
        builder.HasIndex(s => s.StatusName)
            .IsUnique()
            .HasDatabaseName("UQ_MemberStatusTypes_StatusName");

        builder.HasIndex(s => s.DisplayOrder)
            .HasDatabaseName("IX_MemberStatusTypes_DisplayOrder");

        // Relación con Members (opcional, si decides hacer STATUS una FK)
        // builder.HasMany(s => s.Members)
        //     .WithOne()
        //     .HasForeignKey(m => m.StatusId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
