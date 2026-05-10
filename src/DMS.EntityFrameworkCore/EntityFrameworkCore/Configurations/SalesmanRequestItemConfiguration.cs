using DMS.SalesmanRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanRequestItemConfiguration : IEntityTypeConfiguration<SalesmanRequestItem>
{
    public void Configure(EntityTypeBuilder<SalesmanRequestItem> builder)
    {
        builder.ToTable("SalesmanRequestItems");

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)");
    }
}
