using Abp.Application.Services.Dto;
using DMS.Routes;
using System;

namespace DMS.Routes.Dto;

public class PagedRouteResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public long? AssignedUserId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public RouteStatus? Status { get; set; }
}
