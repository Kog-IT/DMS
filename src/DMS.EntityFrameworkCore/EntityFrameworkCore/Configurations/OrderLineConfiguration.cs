using DMS.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");

        builder.Property(l => l.ProductName).IsRequired().HasMaxLength(OrderLine.MaxProductNameLength);

        builder.Property(l => l.UnitPrice).HasColumnType("decimal(18,4)");
        builder.Property(l => l.TaxRate).HasColumnType("decimal(18,4)");
        builder.Property(l => l.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(l => l.LineTotal).HasColumnType("decimal(18,4)");

        builder.Property(l => l.DiscountType).HasConversion<int>();

        builder.HasOne<Order>()
            .WithMany(o => o.Lines)
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.OrderId);
    }
}
