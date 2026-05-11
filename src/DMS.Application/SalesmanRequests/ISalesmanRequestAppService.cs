using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.SalesmanRequests.Dto;
using System.Threading.Tasks;

namespace DMS.SalesmanRequests;

public interface ISalesmanRequestAppService
{
    Task<ApiResponse<SalesmanRequestDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<SalesmanRequestDto>>> GetAllAsync(PagedSalesmanRequestResultRequestDto input);
    Task<ApiResponse<SalesmanRequestDto>> CreateAsync(CreateSalesmanRequestDto input);
    Task<ApiResponse<SalesmanRequestDto>> UpdateAsync(UpdateSalesmanRequestDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input);
    Task<ApiResponse<object>> RejectAsync(RejectSalesmanRequestDto input);
}
