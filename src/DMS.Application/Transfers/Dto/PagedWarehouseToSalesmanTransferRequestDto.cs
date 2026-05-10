using Abp.Application.Services.Dto;

namespace DMS.Transfers.Dto;

public class PagedWarehouseToSalesmanTransferRequestDto : PagedResultRequestDto
{
    public int? WarehouseId { get; set; }
    public int? SalesmanId { get; set; }
    public int? Status { get; set; }
}
