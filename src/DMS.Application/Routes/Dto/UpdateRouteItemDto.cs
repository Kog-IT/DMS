namespace DMS.Routes.Dto;

/// <summary>
/// Used when updating a route's items. If Id is null, the item is new; if set, it matches an existing RouteItem.
/// </summary>
public class UpdateRouteItemDto
{
    public int? Id { get; set; }
    public int CustomerId { get; set; }
    public int OrderIndex { get; set; }
    public int? PlannedDurationMinutes { get; set; }
}
