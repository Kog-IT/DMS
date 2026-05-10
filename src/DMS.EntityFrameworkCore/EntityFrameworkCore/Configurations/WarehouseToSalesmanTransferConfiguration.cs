using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseToSalesmanTransferConfiguration : IEntityTypeConfiguration<WarehouseToSalesmanTransfer>
{
    public void Configure(EntityTypeBuilder<WarehouseToSalesmanTransfer> builder)
    {
        builder.ToTable("WarehouseToSalesmanTransfers");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);
    }
}
