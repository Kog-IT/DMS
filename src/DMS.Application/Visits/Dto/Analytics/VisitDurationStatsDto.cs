namespace DMS.Visits.Dto.Analytics;

public class VisitDurationStatsDto
{
    public double AverageDurationMinutes { get; set; }
    public int MinDurationMinutes { get; set; }
    public int MaxDurationMinutes { get; set; }
    public int TotalVisitsWithDuration { get; set; }
}
