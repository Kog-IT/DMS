using Abp.Application.Services.Dto;

namespace DMS.Routes.Dto;

public class RouteItemDto : EntityDto<int>
{
    public int RouteId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int? VisitId { get; set; }
    public int OrderIndex { get; set; }
    public int? PlannedDurationMinutes { get; set; }
}
