using Abp.Domain.Entities;

namespace DMS.PriceLists;

public class PriceListItem : Entity<int>
{
    public int PriceListId { get; set; }
    public int ProductId { get; set; }
    public decimal MinQuantity { get; set; } = 1m;
    public decimal Price { get; set; }

    public virtual PriceList PriceList { get; set; }
}
