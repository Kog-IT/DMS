using Abp.Application.Services;
using DMS.Companies.Dto;

namespace DMS.Companies;

public interface ICompanyAppService : IAsyncCrudAppService<
    CompanyDto,
    int,
    PagedCompanyResultRequestDto,
    CreateCompanyDto,
    UpdateCompanyDto>
{
}
