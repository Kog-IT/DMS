using Abp.Application.Services.Dto;

namespace DMS.Transfers.Dto;

public class PagedWarehouseToWarehouseTransferRequestDto : PagedResultRequestDto
{
    public int? FromWarehouseId { get; set; }
    public int? ToWarehouseId { get; set; }
    public int? Status { get; set; }
}
