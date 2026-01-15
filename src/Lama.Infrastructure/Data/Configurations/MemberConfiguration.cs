using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Member
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Order)
            .IsRequired();

        builder.Property(m => m.CompleteNames)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName(" Complete Names");

        builder.Property(m => m.Dama)
            .HasDefaultValue(false);

        builder.Property(m => m.CountryBirth)
            .HasMaxLength(100)
            .HasColumnName("Country Birth");

        builder.Property(m => m.InLamaSince)
            .HasColumnName("In Lama Since");

        builder.Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("STATUS");
            // FK a MemberStatusTypes.StatusName en DbContext OnModelCreating()

        builder.Property(m => m.IsEligible)
            .HasDefaultValue(true)
            .HasColumnName("is_eligible");

        builder.Property(m => m.Continent)
            .HasMaxLength(100);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(m => m.Chapter)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Vehicles)
            .WithOne(v => v.Member)
            .HasForeignKey(v => v.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Attendances)
            .WithOne(a => a.Member)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
