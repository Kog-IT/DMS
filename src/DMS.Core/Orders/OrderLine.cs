using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Orders;

public class OrderLine : Entity<int>, IMustHaveTenant
{
    public const int MaxProductNameLength = 256;

    public int TenantId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    [Required]
    [StringLength(MaxProductNameLength)]
    public string ProductName { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int Quantity { get; set; }
    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; } = 0;
    public decimal LineTotal { get; set; }
    public bool IsBackOrder { get; set; } = false;
}
