using System;

namespace DMS.Routes.Dto;

public class OptimizedRouteItemDto
{
    public int RouteItemId { get; set; }
    public int OrderIndex { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>Distance in km from the previous stop (or rep start location for the first stop).</summary>
    public double DistanceFromPreviousKm { get; set; }

    /// <summary>Effective visit duration — RouteItem.PlannedDurationMinutes if set, otherwise tenant default.</summary>
    public int PlannedDurationMinutes { get; set; }

    /// <summary>Estimated arrival time at this stop, accounting for all preceding travel and visit durations.</summary>
    public DateTime EstimatedArrivalTime { get; set; }
}
