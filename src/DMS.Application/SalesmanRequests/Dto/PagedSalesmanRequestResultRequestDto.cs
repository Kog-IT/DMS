using Abp.Application.Services.Dto;
using System;

namespace DMS.SalesmanRequests.Dto;

public class PagedSalesmanRequestResultRequestDto : PagedResultRequestDto
{
    public int? SalesmanId { get; set; }
    public int? WarehouseId { get; set; }
    public int? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
