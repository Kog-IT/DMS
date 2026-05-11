using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanToSalesmanTransferConfiguration : IEntityTypeConfiguration<SalesmanToSalesmanTransfer>
{
    public void Configure(EntityTypeBuilder<SalesmanToSalesmanTransfer> builder)
    {
        builder.ToTable("SalesmanToSalesmanTransfers");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);
    }
}
