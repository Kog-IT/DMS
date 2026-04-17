using System.ComponentModel.DataAnnotations;

namespace DMS.PriceLists.Dto;

public class AssignPriceListDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PriceListId { get; set; }
}
