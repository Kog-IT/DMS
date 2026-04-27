using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Suppliers.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Suppliers;

public interface ISupplierAppService
{
    Task<ApiResponse<SupplierDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<SupplierDto>>> GetAllAsync(PagedSupplierResultRequestDto input);
    Task<ApiResponse<SupplierDto>> CreateAsync(CreateSupplierDto input);
    Task<ApiResponse<SupplierDto>> UpdateAsync(UpdateSupplierDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
}
