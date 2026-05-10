using Abp.Application.Services.Dto;
using System;

namespace DMS.Dispatches.Dto;

public class PagedPlannedDispatchResultRequestDto : PagedResultRequestDto
{
    public int? SalesmanId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? Status { get; set; }
}
