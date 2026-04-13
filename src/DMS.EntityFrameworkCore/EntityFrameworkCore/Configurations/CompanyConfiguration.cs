using DMS.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(Company.MaxNameLength);

        builder.Property(c => c.TaxNumber)
            .HasMaxLength(Company.MaxTaxNumberLength);

        builder.Property(c => c.Address)
            .HasMaxLength(Company.MaxAddressLength);

        builder.Property(c => c.Phone)
            .HasMaxLength(Company.MaxPhoneLength);

        builder.Property(c => c.Email)
            .HasMaxLength(Company.MaxEmailLength);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.IsActive);
    }
}
