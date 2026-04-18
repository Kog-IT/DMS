using Abp.Application.Services;
using DMS.Routes.Dto;
using System.Threading.Tasks;

namespace DMS.Routes;

public interface IRouteAppService : IAsyncCrudAppService<
    RouteDto,
    int,
    PagedRouteResultRequestDto,
    CreateRouteDto,
    UpdateRouteDto>
{
    Task ActivateAsync(int routeId);
    Task<OptimizeRouteResultDto> OptimizeRouteAsync(OptimizeRouteInputDto input);
}
