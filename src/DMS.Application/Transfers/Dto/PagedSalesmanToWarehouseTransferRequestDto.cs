using Abp.Application.Services.Dto;

namespace DMS.Transfers.Dto;

public class PagedSalesmanToWarehouseTransferRequestDto : PagedResultRequestDto
{
    public int? SalesmanId { get; set; }
    public int? WarehouseId { get; set; }
    public int? Status { get; set; }
}
