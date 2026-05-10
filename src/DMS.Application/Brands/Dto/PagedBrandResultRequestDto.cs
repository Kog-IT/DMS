using Abp.Application.Services.Dto;

namespace DMS.Brands.Dto;

public class PagedBrandResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
}
