using DMS.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(Customer.MaxCodeLength);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(Customer.MaxNameLength);

        builder.Property(c => c.Address)
            .HasMaxLength(Customer.MaxAddressLength);

        builder.Property(c => c.Phone)
            .HasMaxLength(Customer.MaxPhoneLength);

        builder.Property(c => c.Email)
            .HasMaxLength(Customer.MaxEmailLength);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.Classification)
            .HasConversion<int>()
            .HasDefaultValue(CustomerClassification.Unclassified);

        builder.HasIndex(c => new { c.TenantId, c.Code }).IsUnique();
        builder.HasIndex(c => c.IsActive);

        builder.Property(c => c.CreditLimit)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(c => c.CreditEnabled)
            .HasDefaultValue(false);
    }
}
