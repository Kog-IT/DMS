using System;

namespace DMS.Visits.Dto.Analytics;

public class VisitAnalyticsFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public long? AssignedUserId { get; set; }
    public int? RouteId { get; set; }
}
