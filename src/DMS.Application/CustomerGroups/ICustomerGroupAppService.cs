using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.CustomerGroups.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.CustomerGroups;

public interface ICustomerGroupAppService
{
    Task<ApiResponse<CustomerGroupDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<CustomerGroupDto>>> GetAllAsync(PagedCustomerGroupResultRequestDto input);
    Task<ApiResponse<CustomerGroupDto>> CreateAsync(CreateCustomerGroupDto input);
    Task<ApiResponse<CustomerGroupDto>> UpdateAsync(UpdateCustomerGroupDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
    Task<ApiResponse<object>> UpdateTaxExemptedAsync(UpdateCustomerGroupTaxExemptedDto input);
}
