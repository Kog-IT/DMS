using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanToWarehouseTransferConfiguration : IEntityTypeConfiguration<SalesmanToWarehouseTransfer>
{
    public void Configure(EntityTypeBuilder<SalesmanToWarehouseTransfer> builder)
    {
        builder.ToTable("SalesmanToWarehouseTransfers");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);
    }
}
