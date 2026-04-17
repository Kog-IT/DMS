using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Products;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMS.EntityFrameworkCore.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(Product.MaxNameLength);

            builder.Property(p => p.Description)
                .HasMaxLength(Product.MaxDescriptionLength);


            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.TaxRate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0m);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 

          
            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.TenantId);
        }
    }
}
