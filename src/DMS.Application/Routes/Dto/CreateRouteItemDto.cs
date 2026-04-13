using Abp.AutoMapper;
using DMS.Routes;

namespace DMS.Routes.Dto;

[AutoMapTo(typeof(RouteItem))]
public class CreateRouteItemDto
{
    public int CustomerId { get; set; }
    public int OrderIndex { get; set; }
    public int? PlannedDurationMinutes { get; set; }
}
