using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseToWarehouseTransferItemConfiguration : IEntityTypeConfiguration<WarehouseToWarehouseTransferItem>
{
    public void Configure(EntityTypeBuilder<WarehouseToWarehouseTransferItem> builder)
    {
        builder.ToTable("WarehouseToWarehouseTransferItems");

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)");
    }
}
