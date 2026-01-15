using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de mapeo fluent para la entidad Vehicle
/// </summary>
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.MotorcycleData)
            .IsRequired()
            .HasColumnName(" Motorcycle Data");

        builder.Property(v => v.LicPlate)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("Lic Plate");

        builder.HasIndex(v => v.LicPlate)
            .IsUnique();

        builder.Property(v => v.Trike)
            .HasDefaultValue(false)
            .HasColumnName("Trike");

        builder.Property(v => v.Photography)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("PENDING");

        builder.Property(v => v.StartYearEvidenceUrl)
            .HasColumnName("StartYearEvidenceUrl");

        builder.Property(v => v.CutOffEvidenceUrl)
            .HasColumnName("CutOffEvidenceUrl");

        builder.Property(v => v.StartYearEvidenceValidatedAt)
            .HasColumnName("StartYearEvidenceValidatedAt");

        builder.Property(v => v.CutOffEvidenceValidatedAt)
            .HasColumnName("CutOffEvidenceValidatedAt");

        builder.Property(v => v.EvidenceValidatedBy)
            .HasColumnName("EvidenceValidatedBy");

        builder.Property(v => v.OdometerUnit)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Miles")
            .HasColumnName("OdometerUnit");

        builder.Property(v => v.StartingOdometer)
            .HasColumnName("Starting Odometer");

        builder.Property(v => v.FinalOdometer)
            .HasColumnName("Final Odometer");

        builder.Property(v => v.StartingOdometerDate)
            .HasColumnName("Starting Odometer Date");

        builder.Property(v => v.FinalOdometerDate)
            .HasColumnName("Final Odometer Date");

        builder.Property(v => v.IsActiveForChampionship)
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(v => v.Member)
            .WithMany(m => m.Vehicles)
            .HasForeignKey(v => v.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.ValidatedByMember)
            .WithMany()
            .HasForeignKey(v => v.EvidenceValidatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(v => v.Attendances)
            .WithOne(a => a.Vehicle)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
