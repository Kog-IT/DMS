using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Routes.Dto;

public class OptimizeRouteInputDto
{
    [Required]
    public int RouteId { get; set; }

    [Required]
    public double RepLatitude { get; set; }

    [Required]
    public double RepLongitude { get; set; }

    /// <summary>
    /// When the rep starts the route. Defaults to DateTime.Now if not provided.
    /// Used to calculate EstimatedArrivalTime per stop.
    /// </summary>
    public DateTime? StartTime { get; set; }
}
