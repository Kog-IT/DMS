namespace DMS.Visits.Dto;

public class SyncVisitResultDto
{
    public string ExternalId { get; set; }
    public bool Success { get; set; }
    public int? VisitId { get; set; }
    public string Error { get; set; }
}
