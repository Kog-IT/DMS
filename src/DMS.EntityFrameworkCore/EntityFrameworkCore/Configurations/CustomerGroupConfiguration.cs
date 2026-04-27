using DMS.CustomerGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class CustomerGroupConfiguration : IEntityTypeConfiguration<CustomerGroup>
{
    public void Configure(EntityTypeBuilder<CustomerGroup> builder)
    {
        builder.ToTable("CustomerGroups");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CustomerGroup.MaxNameLength);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsTaxExempted)
            .HasDefaultValue(false);
    }
}
