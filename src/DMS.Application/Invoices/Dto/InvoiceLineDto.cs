using DMS.Orders;

namespace DMS.Invoices.Dto;

public class InvoiceLineDto
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int Quantity { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal LineTotal { get; set; }
}
