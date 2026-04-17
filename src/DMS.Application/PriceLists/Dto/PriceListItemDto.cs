namespace DMS.PriceLists.Dto;

public class PriceListItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal MinQuantity { get; set; }
    public decimal Price { get; set; }
}
