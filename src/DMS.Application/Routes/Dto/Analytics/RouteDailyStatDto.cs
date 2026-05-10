using System;

namespace DMS.Routes.Dto.Analytics;

public class RouteDailyStatDto
{
    public DateTime Date { get; set; }
    public int TotalRoutes { get; set; }
    public int CompletedRoutes { get; set; }
    public int ActiveRoutes { get; set; }
}
