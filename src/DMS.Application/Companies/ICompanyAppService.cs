using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Companies.Dto;
using System.Threading.Tasks;

namespace DMS.Companies;

public interface ICompanyAppService
{
    Task<ApiResponse<CompanyDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<CompanyDto>>> GetAllAsync(PagedCompanyResultRequestDto input);
    Task<ApiResponse<CompanyDto>> CreateAsync(CreateCompanyDto input);
    Task<ApiResponse<CompanyDto>> UpdateAsync(UpdateCompanyDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
