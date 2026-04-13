using DMS.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");

        builder.Property(v => v.Notes).HasMaxLength(Visit.MaxNotesLength);
        builder.Property(v => v.SkipReason).HasMaxLength(Visit.MaxSkipReasonLength);
        builder.Property(v => v.NoSaleReason).HasMaxLength(Visit.MaxNoSaleReasonLength);
        builder.Property(v => v.ExternalId).HasMaxLength(Visit.MaxExternalIdLength);

        builder.Property(v => v.Status).HasConversion<int>();

        builder.HasMany(v => v.Photos)
            .WithOne(p => p.Visit)
            .HasForeignKey(p => p.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.TenantId, v.AssignedUserId });
        builder.HasIndex(v => new { v.TenantId, v.Status });
        builder.HasIndex(v => v.PlannedDate);
        builder.HasIndex(v => v.CustomerId);
    }
}
