using DMS.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(Brand.MaxNameLength);

        builder.Property(b => b.Name_EN)
            .HasMaxLength(Brand.MaxNameLength);
    }
}
