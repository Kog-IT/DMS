namespace DMS.PriceLists.Dto;

public class PriceListAssignmentDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PriceListId { get; set; }
    public string PriceListName { get; set; }
}
