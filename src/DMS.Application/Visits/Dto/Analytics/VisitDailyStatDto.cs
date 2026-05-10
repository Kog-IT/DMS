using System;

namespace DMS.Visits.Dto.Analytics;

public class VisitDailyStatDto
{
    public DateTime Date { get; set; }
    public int Planned { get; set; }
    public int Completed { get; set; }
    public int Skipped { get; set; }
}
