namespace DMS.PriceLists;

public class PriceResolutionResult
{
    public decimal Price { get; set; }
    public bool IsBasePriceFallback { get; set; }
    public int? PriceListId { get; set; }
}
