using DMS.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseProductConfiguration : IEntityTypeConfiguration<WarehouseProduct>
{
    public void Configure(EntityTypeBuilder<WarehouseProduct> builder)
    {
        builder.ToTable("WarehouseProducts");

        builder.Property(wp => wp.Quantity)
            .HasColumnType("decimal(18,4)");

        builder.Property(wp => wp.WeightPerKG)
            .HasColumnType("decimal(18,4)");

        builder.HasIndex(wp => new { wp.WarehouseId, wp.ProductId });
    }
}
