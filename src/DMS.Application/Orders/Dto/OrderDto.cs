using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using DMS.Orders;

namespace DMS.Orders.Dto;

public class OrderDto : EntityDto<int>
{
    public int CustomerId { get; set; }
    public int? VisitId { get; set; }
    public long AssignedUserId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public string Notes { get; set; }
    public DiscountType OrderDiscountType { get; set; }
    public decimal OrderDiscountValue { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal OrderDiscountAmount { get; set; }
    public decimal Total { get; set; }
    public string RejectionReason { get; set; }
    public List<OrderLineDto> Lines { get; set; } = new();
}
