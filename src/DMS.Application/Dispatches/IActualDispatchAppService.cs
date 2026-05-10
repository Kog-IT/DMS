using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Dispatches.Dto;
using System.Threading.Tasks;

namespace DMS.Dispatches;

public interface IActualDispatchAppService
{
    Task<ApiResponse<ActualDispatchDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<ActualDispatchDto>>> GetAllAsync(PagedActualDispatchResultRequestDto input);
    Task<ApiResponse<ActualDispatchDto>> CreateAsync(CreateActualDispatchDto input);
    Task<ApiResponse<ActualDispatchDto>> UpdateAsync(UpdateActualDispatchDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
