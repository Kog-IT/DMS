namespace DMS.Routes.Dto.Analytics;

public class RouteEfficiencyDto
{
    public int RouteId { get; set; }
    public string RouteName { get; set; }
    public int PlannedStops { get; set; }
    public int CompletedVisits { get; set; }
    public int SkippedVisits { get; set; }
    public double EfficiencyRate { get; set; }
}
