using Abp.Application.Services.Dto;
using DMS.Brands.Dto;
using DMS.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Brands;

public interface IBrandAppService
{
    Task<ApiResponse<BrandDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<BrandDto>>> GetAllAsync(PagedBrandResultRequestDto input);
    Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto input);
    Task<ApiResponse<BrandDto>> UpdateAsync(UpdateBrandDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
}
