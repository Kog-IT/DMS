using Abp.Application.Services.Dto;

namespace DMS.Companies.Dto;

public class PagedCompanyResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
}
