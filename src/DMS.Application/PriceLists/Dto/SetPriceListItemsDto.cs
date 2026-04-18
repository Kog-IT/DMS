using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.PriceLists.Dto;

public class SetPriceListItemsDto
{
    [Required]
    public int PriceListId { get; set; }

    [Required]
    public List<PriceListItemInputDto> Items { get; set; } = new();
}

public class PriceListItemInputDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, double.MaxValue)]
    public decimal MinQuantity { get; set; } = 1m;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
