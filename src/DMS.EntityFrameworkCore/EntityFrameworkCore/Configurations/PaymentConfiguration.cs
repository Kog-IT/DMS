using DMS.Invoices;
using DMS.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.Property(p => p.ReceiptNumber).IsRequired().HasMaxLength(Payment.MaxReceiptNumberLength);
        builder.Property(p => p.Notes).HasMaxLength(Payment.MaxNotesLength);
        builder.Property(p => p.ReceiptFilePath).HasMaxLength(Payment.MaxReceiptFilePathLength);
        builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,4)");

        builder.HasOne<Invoice>()
            .WithMany()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.TenantId, p.ReceiptNumber }).IsUnique();
        builder.HasIndex(p => new { p.TenantId, p.InvoiceId });
    }
}
