using DMS.ProductGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class ProductGroupConfiguration : IEntityTypeConfiguration<ProductGroup>
{
    public void Configure(EntityTypeBuilder<ProductGroup> builder)
    {
        builder.ToTable("ProductGroups");

        builder.Property(pg => pg.Name)
            .IsRequired()
            .HasMaxLength(ProductGroup.MaxNameLength);

        builder.Property(pg => pg.Name_EN)
            .HasMaxLength(ProductGroup.MaxNameLength);
    }
}
