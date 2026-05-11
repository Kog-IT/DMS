using Abp.Application.Services.Dto;

namespace DMS.ProductGroups.Dto;

public class PagedProductGroupResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
}
