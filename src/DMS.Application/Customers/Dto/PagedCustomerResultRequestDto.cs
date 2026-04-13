using Abp.Application.Services.Dto;

namespace DMS.Customers.Dto;

public class PagedCustomerResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
}
