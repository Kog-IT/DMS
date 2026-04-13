using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Routes;

namespace DMS.Routes.Dto;

[AutoMapFrom(typeof(RouteItem))]
public class RouteItemDto : EntityDto<int>
{
    public int RouteId { get; set; }
    public int CustomerId { get; set; }
    public int OrderIndex { get; set; }
    public int? PlannedDurationMinutes { get; set; }
}
