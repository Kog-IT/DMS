using DMS.Invoices;
using DMS.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(Invoice.MaxInvoiceNumberLength);
        builder.Property(i => i.CustomerName).HasMaxLength(Invoice.MaxCustomerNameLength);
        builder.Property(i => i.CustomerAddress).HasMaxLength(Invoice.MaxCustomerAddressLength);
        builder.Property(i => i.Notes).HasMaxLength(Invoice.MaxNotesLength);
        builder.Property(i => i.VoidReason).HasMaxLength(Invoice.MaxVoidReasonLength);

        builder.Property(i => i.SubTotal).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TaxTotal).HasColumnType("decimal(18,4)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.Total).HasColumnType("decimal(18,4)");
        builder.Property(i => i.PaidAmount).HasColumnType("decimal(18,4)");

        builder.Property(i => i.Status).HasConversion<int>();

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
        builder.HasIndex(i => new { i.TenantId, i.OrderId }).IsUnique(); // one invoice per order per tenant
        builder.HasIndex(i => new { i.TenantId, i.Status });
    }
}
