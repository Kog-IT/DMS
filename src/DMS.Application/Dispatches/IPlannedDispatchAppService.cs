using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Dispatches.Dto;
using System.Threading.Tasks;

namespace DMS.Dispatches;

public interface IPlannedDispatchAppService
{
    Task<ApiResponse<PlannedDispatchDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<PlannedDispatchDto>>> GetAllAsync(PagedPlannedDispatchResultRequestDto input);
    Task<ApiResponse<PlannedDispatchDto>> CreateAsync(CreatePlannedDispatchDto input);
    Task<ApiResponse<PlannedDispatchDto>> UpdateAsync(UpdatePlannedDispatchDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
