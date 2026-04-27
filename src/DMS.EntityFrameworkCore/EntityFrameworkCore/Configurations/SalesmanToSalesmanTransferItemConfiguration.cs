using DMS.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanToSalesmanTransferItemConfiguration : IEntityTypeConfiguration<SalesmanToSalesmanTransferItem>
{
    public void Configure(EntityTypeBuilder<SalesmanToSalesmanTransferItem> builder)
    {
        builder.ToTable("SalesmanToSalesmanTransferItems");

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)");
    }
}
