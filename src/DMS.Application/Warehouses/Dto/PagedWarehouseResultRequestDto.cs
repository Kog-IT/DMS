using Abp.Application.Services.Dto;

namespace DMS.Warehouses.Dto;

public class PagedWarehouseResultRequestDto : PagedResultRequestDto
{
    public string? Keyword { get; set; }
    public int? GovernorateId { get; set; }
    public int? CityId { get; set; }
    public bool? IsActive { get; set; }
}
