using DMS.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods");

        builder.Property(p => p.Name).IsRequired().HasMaxLength(PaymentMethod.MaxNameLength);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(PaymentMethod.MaxCodeLength);

        builder.HasIndex(p => new { p.TenantId, p.Code }).IsUnique();
    }
}
