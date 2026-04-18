using System;
using System.Collections.Generic;

namespace DMS.Routes.Dto;

public class OptimizeRouteResultDto
{
    public int RouteId { get; set; }
    public List<OptimizedRouteItemDto> Items { get; set; } = new();

    /// <summary>Sum of all segment distances in km.</summary>
    public double TotalDistanceKm { get; set; }

    /// <summary>Sum of all travel minutes and visit durations.</summary>
    public int TotalDurationMinutes { get; set; }

    /// <summary>StartTime + TotalDurationMinutes.</summary>
    public DateTime EstimatedEndTime { get; set; }
}
