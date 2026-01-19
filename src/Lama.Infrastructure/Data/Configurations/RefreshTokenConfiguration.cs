using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lama.Domain.Entities;

namespace Lama.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para RefreshToken
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.IdentityUserId)
            .IsRequired();

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.RevokedAt)
            .IsRequired(false);

        builder.Property(r => r.ReplacedByTokenId)
            .IsRequired(false);

        builder.Property(r => r.RevocationReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedByIp)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(r => r.UserAgent)
            .HasMaxLength(500)
            .IsRequired(false);

        // Relación con IdentityUser
        builder.HasOne(r => r.IdentityUser)
            .WithMany()
            .HasForeignKey(r => r.IdentityUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(r => r.TokenHash)
            .HasDatabaseName("IX_RefreshTokens_TokenHash");

        builder.HasIndex(r => r.IdentityUserId)
            .HasDatabaseName("IX_RefreshTokens_IdentityUserId");

        builder.HasIndex(r => r.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

        builder.HasIndex(r => new { r.TenantId, r.IdentityUserId })
            .HasDatabaseName("IX_RefreshTokens_TenantId_IdentityUserId");
    }
}
