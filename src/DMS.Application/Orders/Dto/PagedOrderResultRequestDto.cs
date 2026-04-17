using System;
using Abp.Application.Services.Dto;
using DMS.Orders;

namespace DMS.Orders.Dto;

public class PagedOrderResultRequestDto : PagedResultRequestDto
{
    public int? CustomerId { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
