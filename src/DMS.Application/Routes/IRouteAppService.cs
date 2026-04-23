using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Routes.Dto;
using System.Threading.Tasks;

namespace DMS.Routes;

public interface IRouteAppService
{
    Task<ApiResponse<RouteDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<RouteDto>>> GetAllAsync(PagedRouteResultRequestDto input);
    Task<ApiResponse<RouteDto>> CreateAsync(CreateRouteDto input);
    Task<ApiResponse<RouteDto>> UpdateAsync(UpdateRouteDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(int routeId);
    Task<ApiResponse<OptimizeRouteResultDto>> OptimizeRouteAsync(OptimizeRouteInputDto input);
}
