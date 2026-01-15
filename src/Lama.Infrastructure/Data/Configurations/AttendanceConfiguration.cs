using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Attendance
/// </summary>
public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendance");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("PENDING");

        builder.Property(a => a.PointsPerEvent)
            .HasColumnName("Points per event");

        builder.Property(a => a.PointsPerDistance)
            .HasColumnName("Points per Distance");

        builder.Property(a => a.PointsAwardedPerMember)
            .HasColumnName("Points awarded per member");

        builder.Property(a => a.VisitorClass)
            .HasMaxLength(50)
            .HasColumnName("Visitor Class");

        builder.Property(a => a.ConfirmedAt);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint: un miembro solo puede asistir una vez a un evento
        builder.HasIndex(a => new { a.EventId, a.MemberId })
            .IsUnique()
            .HasDatabaseName("UQ_Attendance_MemberEvent");

        builder.HasOne(a => a.Event)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Member)
            .WithMany(m => m.Attendances)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Vehicle)
            .WithMany(v => v.Attendances)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ConfirmedByMember)
            .WithMany()
            .HasForeignKey(a => a.ConfirmedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
