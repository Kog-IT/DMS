using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DMS.Orders;

namespace DMS.Orders.Dto;

public class CreateOrderDto
{
    [Required]
    public int CustomerId { get; set; }

    public int? VisitId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [StringLength(Order.MaxNotesLength)]
    public string Notes { get; set; }

    public DiscountType OrderDiscountType { get; set; } = DiscountType.None;

    [Range(0, double.MaxValue)]
    public decimal OrderDiscountValue { get; set; } = 0;

    [Required]
    [MinLength(1)]
    public List<CreateOrderLineDto> Lines { get; set; } = new();
}
