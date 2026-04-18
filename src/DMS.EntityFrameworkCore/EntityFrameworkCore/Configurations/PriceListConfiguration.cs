using DMS.PriceLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable("PriceLists");

        builder.Property(p => p.Name).IsRequired().HasMaxLength(PriceList.MaxNameLength);
        builder.Property(p => p.Description).HasMaxLength(PriceList.MaxDescriptionLength);
        builder.Property(p => p.ForClassification).HasConversion<int?>();

        builder.HasIndex(p => new { p.TenantId, p.ForClassification });
        builder.HasIndex(p => new { p.TenantId, p.IsActive });
    }
}
