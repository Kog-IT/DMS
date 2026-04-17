using DMS.Orders;

namespace DMS.Orders.Dto;

public class OrderLineDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int Quantity { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal LineTotal { get; set; }
    public bool IsBackOrder { get; set; }
    public bool IsBasePriceFallback { get; set; }
}
