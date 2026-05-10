using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.MultiTenancy.Dto;
using System.Threading.Tasks;

namespace DMS.MultiTenancy;

public interface ITenantAppService
{
    Task<ApiResponse<TenantDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<TenantDto>>> GetAllAsync(PagedTenantResultRequestDto input);
    Task<ApiResponse<TenantDto>> CreateAsync(CreateTenantDto input);
    Task<ApiResponse<TenantDto>> UpdateAsync(TenantDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<TenantDto>> UpdateTenantImageAsync(UpdateTenantImageDto input);
}
