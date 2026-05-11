using Abp.Application.Services.Dto;

namespace DMS.Transfers.Dto;

public class PagedSalesmanToSalesmanTransferRequestDto : PagedResultRequestDto
{
    public int? FromSalesmanId { get; set; }
    public int? ToSalesmanId { get; set; }
    public int? Status { get; set; }
}
