using Abp.Application.Services.Dto;

namespace DMS.Payments.Dto;

public class PagedPaymentMethodResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
}
