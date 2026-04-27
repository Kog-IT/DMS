using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanToWarehouseTransferItemConfiguration : IEntityTypeConfiguration<SalesmanToWarehouseTransferItem>
{
    public void Configure(EntityTypeBuilder<SalesmanToWarehouseTransferItem> builder)
    {
        builder.ToTable("SalesmanToWarehouseTransferItems");

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)");
    }
}
