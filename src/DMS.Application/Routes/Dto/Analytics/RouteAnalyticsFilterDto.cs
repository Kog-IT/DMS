using DMS.Routes;
using System;

namespace DMS.Routes.Dto.Analytics;

public class RouteAnalyticsFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public long? AssignedUserId { get; set; }
    public RouteStatus? Status { get; set; }
}
