namespace DMS.Routes.Dto.Analytics;

public class RouteSummaryStatsDto
{
    public int TotalRoutes { get; set; }
    public int DraftRoutes { get; set; }
    public int ActiveRoutes { get; set; }
    public int CompletedRoutes { get; set; }
    public double AverageStopsPerRoute { get; set; }
    public double AverageEfficiencyRate { get; set; }
}
