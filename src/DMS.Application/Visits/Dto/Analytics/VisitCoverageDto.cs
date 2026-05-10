namespace DMS.Visits.Dto.Analytics;

public class VisitCoverageDto
{
    public int TotalPlanned { get; set; }
    public int Completed { get; set; }
    public int Skipped { get; set; }
    public int NoSale { get; set; }
    public int InProgress { get; set; }
    public double CoveragePercentage { get; set; }
    public double CompletionRate { get; set; }
}
