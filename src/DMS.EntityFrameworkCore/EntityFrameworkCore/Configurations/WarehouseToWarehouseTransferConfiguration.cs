using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseToWarehouseTransferConfiguration : IEntityTypeConfiguration<WarehouseToWarehouseTransfer>
{
    public void Configure(EntityTypeBuilder<WarehouseToWarehouseTransfer> builder)
    {
        builder.ToTable("WarehouseToWarehouseTransfers");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);
    }
}
