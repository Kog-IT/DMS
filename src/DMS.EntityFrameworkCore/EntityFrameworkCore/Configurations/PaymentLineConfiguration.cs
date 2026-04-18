using DMS.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PaymentLineConfiguration : IEntityTypeConfiguration<PaymentLine>
{
    public void Configure(EntityTypeBuilder<PaymentLine> builder)
    {
        builder.ToTable("PaymentLines");

        builder.Property(p => p.Reference).HasMaxLength(PaymentLine.MaxReferenceLength);
        builder.Property(p => p.Amount).HasColumnType("decimal(18,4)");

        builder.HasOne<Payment>()
            .WithMany(p => p.Lines)
            .HasForeignKey(p => p.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.PaymentMethod)
            .WithMany()
            .HasForeignKey(p => p.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
