using DMS.SalesmanRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanRequestConfiguration : IEntityTypeConfiguration<SalesmanRequest>
{
    public void Configure(EntityTypeBuilder<SalesmanRequest> builder)
    {
        builder.ToTable("SalesmanRequests");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);
    }
}
