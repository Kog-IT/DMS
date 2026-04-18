using DMS.Customers;
using DMS.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(o => o.Notes).HasMaxLength(Order.MaxNotesLength);
        builder.Property(o => o.RejectionReason).HasMaxLength(Order.MaxRejectionReasonLength);

        builder.Property(o => o.SubTotal).HasColumnType("decimal(18,4)");
        builder.Property(o => o.TaxTotal).HasColumnType("decimal(18,4)");
        builder.Property(o => o.OrderDiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(o => o.Total).HasColumnType("decimal(18,4)");
        builder.Property(o => o.OrderDiscountValue).HasColumnType("decimal(18,4)");

        builder.Property(o => o.Status).HasConversion<int>();
        builder.Property(o => o.OrderDiscountType).HasConversion<int>();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => new { o.TenantId, o.CustomerId });
        builder.HasIndex(o => new { o.TenantId, o.Status });
    }
}
