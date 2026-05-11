using Abp.Application.Services.Dto;

namespace DMS.Salesmen.Dto;

public class PagedSalesmanResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public string NationalNumber { get; set; }
    public string Mobile { get; set; }
    public int? WarehouseId { get; set; }
    public bool? IsActive { get; set; }
}
