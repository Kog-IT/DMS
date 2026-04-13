namespace DMS.Visits.Dto;

public class CheckOutDto
{
    public int VisitId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Notes { get; set; }
    public string NoSaleReason { get; set; }
}
