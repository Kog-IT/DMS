using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.ProductGroups.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ProductGroups;

public interface IProductGroupAppService
{
    Task<ApiResponse<ProductGroupDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<ProductGroupDto>>> GetAllAsync(PagedProductGroupResultRequestDto input);
    Task<ApiResponse<ProductGroupDto>> CreateAsync(CreateProductGroupDto input);
    Task<ApiResponse<ProductGroupDto>> UpdateAsync(UpdateProductGroupDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
}
