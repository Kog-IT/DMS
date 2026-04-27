using Abp.Application.Services;
using DMS.Common.Dto;
using DMS.Visits.Dto.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Visits;

public interface IVisitAnalyticsAppService : IApplicationService
{
    Task<ApiResponse<VisitCoverageDto>> GetCoverageAsync(VisitAnalyticsFilterDto input);
    Task<ApiResponse<VisitDurationStatsDto>> GetDurationStatsAsync(VisitAnalyticsFilterDto input);
    Task<ApiResponse<List<VisitStatusCountDto>>> GetStatusChartAsync(VisitAnalyticsFilterDto input);
    Task<ApiResponse<List<VisitDailyStatDto>>> GetDailyStatsAsync(VisitAnalyticsFilterDto input);
}
