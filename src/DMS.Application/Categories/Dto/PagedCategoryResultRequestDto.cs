using Abp.Application.Services.Dto;

namespace DMS.Categories.Dto;

public class PagedCategoryResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
}
