using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Event
/// </summary>
public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Order)
            .IsRequired();

        builder.Property(e => e.EventStartDate)
            .IsRequired()
            .HasColumnName("Event Start Date (AAAA/MM/DD)");

        builder.Property(e => e.NameOfTheEvent)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("Name of the event");

        builder.Property(e => e.Class)
            .IsRequired();

        builder.Property(e => e.Mileage)
            .IsRequired()
            .HasColumnName("Mileage");

        builder.Property(e => e.PointsPerEvent)
            .IsRequired()
            .HasColumnName("Points per event");

        builder.Property(e => e.PointsPerDistance)
            .IsRequired()
            .HasColumnName("Points per Distance");

        builder.Property(e => e.PointsAwardedPerMember)
            .IsRequired()
            .HasColumnName("Points awarded per member");

        builder.Property(e => e.StartLocationCountry)
            .HasMaxLength(100);

        builder.Property(e => e.StartLocationContinent)
            .HasMaxLength(100);

        builder.Property(e => e.EndLocationCountry)
            .HasMaxLength(100);

        builder.Property(e => e.EndLocationContinent)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Chapter)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Attendances)
            .WithOne(a => a.Event)
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
