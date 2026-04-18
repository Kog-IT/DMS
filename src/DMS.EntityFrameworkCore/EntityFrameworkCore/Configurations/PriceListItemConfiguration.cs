using DMS.PriceLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.ToTable("PriceListItems");

        builder.Property(i => i.MinQuantity).HasColumnType("decimal(18,4)");
        builder.Property(i => i.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(i => i.PriceList)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.PriceListId, i.ProductId, i.MinQuantity }).IsUnique();
    }
}
