using DMS.PriceLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PriceListAssignmentConfiguration : IEntityTypeConfiguration<PriceListAssignment>
{
    public void Configure(EntityTypeBuilder<PriceListAssignment> builder)
    {
        builder.ToTable("PriceListAssignments");

        builder.HasOne(a => a.PriceList)
            .WithMany(p => p.Assignments)
            .HasForeignKey(a => a.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.TenantId, a.CustomerId }).IsUnique();
    }
}
