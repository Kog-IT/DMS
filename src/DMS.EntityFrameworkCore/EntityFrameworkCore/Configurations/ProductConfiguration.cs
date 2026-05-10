using DMS.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(p => p.Name).IsRequired().HasMaxLength(Product.MaxNameLength);
            builder.Property(p => p.Description).HasMaxLength(Product.MaxDescriptionLength);
            builder.Property(p => p.Code).HasMaxLength(Product.MaxCodeLength);
            builder.Property(p => p.ProductAPI).HasMaxLength(Product.MaxProductAPILength);

            builder.Property(p => p.TaxRate).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
            builder.Property(p => p.Price).HasColumnType("decimal(18,4)");
            builder.Property(p => p.WholesalePrice).HasColumnType("decimal(18,4)");
            builder.Property(p => p.RetailPrice).HasColumnType("decimal(18,4)");
            builder.Property(p => p.VipClientsPrice).HasColumnType("decimal(18,4)");
            builder.Property(p => p.PackSize).HasColumnType("decimal(18,4)");
            builder.Property(p => p.WeightPerKG).HasColumnType("decimal(18,4)");
            builder.Property(p => p.NetWeightPerKG).HasColumnType("decimal(18,4)");
            builder.Property(p => p.TotalPackSize).HasColumnType("decimal(18,4)");
            builder.Property(p => p.TotalWeightPerKG).HasColumnType("decimal(18,4)");
            builder.Property(p => p.TotalNetWeightPerKG).HasColumnType("decimal(18,4)");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.TenantId);
            builder.HasIndex(p => new { p.TenantId, p.Code });
        }
    }
}
