using Lama.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para RankingSnapshot
/// Define índices compuestos para queries de ranking rápidas
/// </summary>
public class RankingSnapshotConfiguration : IEntityTypeConfiguration<RankingSnapshot>
{
    public void Configure(EntityTypeBuilder<RankingSnapshot> builder)
    {
        builder.ToTable("RankingSnapshots");

        // Primary Key
        builder.HasKey(rs => rs.Id);

        // Properties
        builder.Property(rs => rs.TenantId)
            .IsRequired();

        builder.Property(rs => rs.Year)
            .IsRequired();

        builder.Property(rs => rs.ScopeType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(rs => rs.ScopeId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rs => rs.MemberId)
            .IsRequired();

        builder.Property(rs => rs.Rank)
            .HasComment("Posición en el ranking (1 = mejor)");

        builder.Property(rs => rs.TotalPoints)
            .IsRequired();

        builder.Property(rs => rs.TotalMiles)
            .IsRequired();

        builder.Property(rs => rs.EventsCount)
            .IsRequired();

        builder.Property(rs => rs.VisitorClass)
            .HasMaxLength(50);

        builder.Property(rs => rs.LastCalculatedAt)
            .IsRequired();

        builder.Property(rs => rs.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(rs => rs.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Foreign Keys
        builder.HasOne(rs => rs.Member)
            .WithMany()
            .HasForeignKey(rs => rs.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices - Estrategia de consulta: TenantId + Year + ScopeType + ScopeId
        builder.HasIndex(rs => new { rs.TenantId, rs.Year, rs.ScopeType, rs.ScopeId })
            .HasDatabaseName("IX_RankingSnapshots_TenantYear_Scope");

        // Índice para búsqueda rápida de un miembro en un ranking
        builder.HasIndex(rs => new { rs.TenantId, rs.Year, rs.ScopeType, rs.ScopeId, rs.MemberId })
            .HasDatabaseName("UX_RankingSnapshots_TenantYear_Scope_Member")
            .IsUnique();

        // Índice para ordenamiento de ranking (puntos DESC)
        builder.HasIndex(rs => new { rs.TenantId, rs.Year, rs.ScopeType, rs.ScopeId, rs.TotalPoints })
            .HasDatabaseName("IX_RankingSnapshots_Scope_Points");

        // Índice para filtros de LastCalculatedAt (recalcule nocturnos)
        builder.HasIndex(rs => rs.LastCalculatedAt)
            .HasDatabaseName("IX_RankingSnapshots_LastCalculatedAt");

        // Índice para búsquedas por MemberId (dashboard del miembro)
        builder.HasIndex(rs => rs.MemberId)
            .HasDatabaseName("IX_RankingSnapshots_MemberId");

        // Índice para limpieza de datos antiguos
        builder.HasIndex(rs => new { rs.Year, rs.CreatedAt })
            .HasDatabaseName("IX_RankingSnapshots_Year_CreatedAt");
    }
}
