using DMS.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.Property(l => l.ProductName).IsRequired().HasMaxLength(InvoiceLine.MaxProductNameLength);
        builder.Property(l => l.UnitPrice).HasColumnType("decimal(18,4)");
        builder.Property(l => l.TaxRate).HasColumnType("decimal(18,4)");
        builder.Property(l => l.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(l => l.LineTotal).HasColumnType("decimal(18,4)");

        builder.Property(l => l.DiscountType).HasConversion<int>();

        builder.HasOne<Invoice>()
            .WithMany(i => i.Lines)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.InvoiceId);
    }
}
