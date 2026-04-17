namespace DMS.Customers.Dto;

public class ClassificationResultDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public CustomerClassification OldClassification { get; set; }
    public CustomerClassification NewClassification { get; set; }
    public int VisitCount { get; set; }
    public bool Changed => OldClassification != NewClassification;
}
