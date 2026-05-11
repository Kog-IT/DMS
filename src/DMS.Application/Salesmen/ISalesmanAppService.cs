using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Salesmen.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Salesmen;

public interface ISalesmanAppService
{
    Task<ApiResponse<SalesmanDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<SalesmanDto>>> GetAllAsync(PagedSalesmanResultRequestDto input);
    Task<ApiResponse<SalesmanDto>> CreateAsync(CreateSalesmanDto input);
    Task<ApiResponse<SalesmanDto>> UpdateAsync(UpdateSalesmanDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
    Task<ApiResponse<List<SalesmanSelectItemDto>>> GetSelectListAsync();
}
