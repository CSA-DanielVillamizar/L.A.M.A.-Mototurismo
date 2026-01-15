using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para la entidad Evidence
/// </summary>
public class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("Evidences");

        // Clave primaria
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.Property(e => e.CorrelationId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.MemberId)
            .IsRequired();

        builder.Property(e => e.VehicleId)
            .IsRequired();

        builder.Property(e => e.EventId)
            .IsRequired(false); // Null para START_YEAR

        builder.Property(e => e.EvidenceType)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>(); // Enum → string

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>(); // Enum → string

        builder.Property(e => e.PilotPhotoBlobPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.OdometerPhotoBlobPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.OdometerReading)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.OdometerUnit)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Kilometers");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.ReviewedAt)
            .IsRequired(false);

        builder.Property(e => e.ReviewedBy)
            .HasMaxLength(255);

        builder.Property(e => e.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(e => e.AttendanceId)
            .IsRequired(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relaciones
        builder.HasOne(e => e.Member)
            .WithMany()
            .HasForeignKey(e => e.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Vehicle)
            .WithMany()
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Attendance)
            .WithOne()
            .HasForeignKey<Evidence>(e => e.AttendanceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices para queries comunes
        builder.HasIndex(e => new { e.TenantId, e.MemberId })
            .HasDatabaseName("IX_Evidences_TenantId_MemberId");

        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_Evidences_TenantId_Status");

        builder.HasIndex(e => new { e.TenantId, e.EventId })
            .HasDatabaseName("IX_Evidences_TenantId_EventId");

        builder.HasIndex(e => e.CorrelationId)
            .IsUnique()
            .HasDatabaseName("UX_Evidences_CorrelationId");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Evidences_CreatedAt");

        builder.HasIndex(e => new { e.TenantId, e.VehicleId })
            .HasDatabaseName("IX_Evidences_TenantId_VehicleId");
    }
}
