using Abp.Application.Services.Dto;

namespace DMS.CustomerGroups.Dto;

public class PagedCustomerGroupResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
}
