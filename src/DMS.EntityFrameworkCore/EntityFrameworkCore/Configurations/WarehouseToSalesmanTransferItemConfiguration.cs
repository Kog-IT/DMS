using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseToSalesmanTransferItemConfiguration : IEntityTypeConfiguration<WarehouseToSalesmanTransferItem>
{
    public void Configure(EntityTypeBuilder<WarehouseToSalesmanTransferItem> builder)
    {
        builder.ToTable("WarehouseToSalesmanTransferItems");

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)");
    }
}
