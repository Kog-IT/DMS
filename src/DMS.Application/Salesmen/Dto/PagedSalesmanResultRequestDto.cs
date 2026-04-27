using Abp.Application.Services.Dto;

namespace DMS.Salesmen.Dto;

public class PagedSalesmanResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public int? GovernorateId { get; set; }
    public int? CityId { get; set; }
    public int? WarehouseId { get; set; }
    public bool? IsActive { get; set; }
}
