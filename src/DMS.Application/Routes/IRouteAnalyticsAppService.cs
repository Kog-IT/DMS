using Abp.Application.Services;
using DMS.Common.Dto;
using DMS.Routes.Dto.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Routes;

public interface IRouteAnalyticsAppService : IApplicationService
{
    Task<ApiResponse<RouteSummaryStatsDto>> GetSummaryAsync(RouteAnalyticsFilterDto input);
    Task<ApiResponse<List<RouteStatusCountDto>>> GetStatusChartAsync(RouteAnalyticsFilterDto input);
    Task<ApiResponse<List<RouteEfficiencyDto>>> GetEfficiencyAsync(RouteAnalyticsFilterDto input);
    Task<ApiResponse<List<RouteDailyStatDto>>> GetDailyStatsAsync(RouteAnalyticsFilterDto input);
}
