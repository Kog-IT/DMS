using DMS.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(Supplier.MaxNameLength);

        builder.Property(s => s.Code)
            .HasMaxLength(Supplier.MaxCodeLength);

        builder.Property(s => s.Phone)
            .HasMaxLength(Supplier.MaxPhoneLength);

        builder.Property(s => s.Email)
            .HasMaxLength(Supplier.MaxEmailLength);

        builder.Property(s => s.Address)
            .HasMaxLength(Supplier.MaxAddressLength);

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(Supplier.MaxImageUrlLength);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);
    }
}
