using System.ComponentModel.DataAnnotations;
using DMS.Orders;

namespace DMS.Orders.Dto;

public class CreateOrderLineDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;

    [Range(0, double.MaxValue)]
    public decimal DiscountValue { get; set; } = 0;
}
