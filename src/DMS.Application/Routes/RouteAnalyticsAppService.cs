using Abp.Domain.Repositories;
using DMS.Common.Dto;
using DMS.Routes.Dto.Analytics;
using DMS.Visits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Routes;

public class RouteAnalyticsAppService : DMSAppServiceBase, IRouteAnalyticsAppService
{
    private readonly IRepository<Route, int> _routeRepository;
    private readonly IRepository<RouteItem, int> _routeItemRepository;
    private readonly IRepository<Visit, int> _visitRepository;

    public RouteAnalyticsAppService(
        IRepository<Route, int> routeRepository,
        IRepository<RouteItem, int> routeItemRepository,
        IRepository<Visit, int> visitRepository)
    {
        _routeRepository = routeRepository;
        _routeItemRepository = routeItemRepository;
        _visitRepository = visitRepository;
    }

    public async Task<ApiResponse<RouteSummaryStatsDto>> GetSummaryAsync(RouteAnalyticsFilterDto input)
    {
        var routes = await BuildRouteQuery(input).ToListAsync();
        int total = routes.Count;

        var itemCounts = await _routeItemRepository.GetAll()
            .GroupBy(i => i.RouteId)
            .Select(g => new { RouteId = g.Key, Count = g.Count() })
            .ToListAsync();

        double avgStops = itemCounts.Any() ? itemCounts.Average(x => x.Count) : 0;

        var completedRouteIds = routes
            .Where(r => r.Status == RouteStatus.Completed)
            .Select(r => r.Id).ToList();

        double avgEfficiency = 0;
        if (completedRouteIds.Any())
        {
            var visits = await _visitRepository.GetAll()
                .Where(v => v.RouteId.HasValue && completedRouteIds.Contains(v.RouteId.Value))
                .ToListAsync();

            var efficiencies = completedRouteIds.Select(rid =>
            {
                var planned = itemCounts.FirstOrDefault(x => x.RouteId == rid)?.Count ?? 0;
                var completed = visits.Count(v => v.RouteId == rid && v.Status == VisitStatus.Completed);
                return planned == 0 ? 0.0 : (double)completed / planned * 100;
            }).ToList();

            avgEfficiency = efficiencies.Any() ? Math.Round(efficiencies.Average(), 2) : 0;
        }

        var dto = new RouteSummaryStatsDto
        {
            TotalRoutes = total,
            DraftRoutes = routes.Count(r => r.Status == RouteStatus.Draft),
            ActiveRoutes = routes.Count(r => r.Status == RouteStatus.Active),
            CompletedRoutes = routes.Count(r => r.Status == RouteStatus.Completed),
            AverageStopsPerRoute = Math.Round(avgStops, 2),
            AverageEfficiencyRate = avgEfficiency,
        };

        return Ok(dto, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<List<RouteStatusCountDto>>> GetStatusChartAsync(RouteAnalyticsFilterDto input)
    {
        var routes = await BuildRouteQuery(input).ToListAsync();
        int total = routes.Count;

        var result = routes
            .GroupBy(r => r.Status)
            .Select(g => new RouteStatusCountDto
            {
                StatusName = g.Key.ToString(),
                Count = g.Count(),
                Percentage = total == 0 ? 0 : Math.Round((double)g.Count() / total * 100, 2),
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Ok(result, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<List<RouteEfficiencyDto>>> GetEfficiencyAsync(RouteAnalyticsFilterDto input)
    {
        var routes = await BuildRouteQuery(input).ToListAsync();
        var routeIds = routes.Select(r => r.Id).ToList();

        var itemCounts = await _routeItemRepository.GetAll()
            .Where(i => routeIds.Contains(i.RouteId))
            .GroupBy(i => i.RouteId)
            .Select(g => new { RouteId = g.Key, Count = g.Count() })
            .ToListAsync();

        var visits = await _visitRepository.GetAll()
            .Where(v => v.RouteId.HasValue && routeIds.Contains(v.RouteId.Value))
            .ToListAsync();

        var result = routes.Select(r =>
        {
            int planned = itemCounts.FirstOrDefault(x => x.RouteId == r.Id)?.Count ?? 0;
            int completed = visits.Count(v => v.RouteId == r.Id && v.Status == VisitStatus.Completed);
            int skipped = visits.Count(v => v.RouteId == r.Id && v.Status == VisitStatus.Skipped);

            return new RouteEfficiencyDto
            {
                RouteId = r.Id,
                RouteName = r.Name,
                PlannedStops = planned,
                CompletedVisits = completed,
                SkippedVisits = skipped,
                EfficiencyRate = planned == 0 ? 0 : Math.Round((double)completed / planned * 100, 2),
            };
        })
        .OrderByDescending(x => x.EfficiencyRate)
        .ToList();

        return Ok(result, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<List<RouteDailyStatDto>>> GetDailyStatsAsync(RouteAnalyticsFilterDto input)
    {
        var routes = await BuildRouteQuery(input).ToListAsync();

        var result = routes
            .GroupBy(r => r.PlannedDate.Date)
            .Select(g => new RouteDailyStatDto
            {
                Date = g.Key,
                TotalRoutes = g.Count(),
                CompletedRoutes = g.Count(r => r.Status == RouteStatus.Completed),
                ActiveRoutes = g.Count(r => r.Status == RouteStatus.Active),
            })
            .OrderBy(x => x.Date)
            .ToList();

        return Ok(result, L("SuccessfullyRetrieved"));
    }

    private IQueryable<Route> BuildRouteQuery(RouteAnalyticsFilterDto input)
    {
        var query = _routeRepository.GetAll();

        if (input.DateFrom.HasValue)
            query = query.Where(r => r.PlannedDate >= input.DateFrom.Value);

        if (input.DateTo.HasValue)
            query = query.Where(r => r.PlannedDate <= input.DateTo.Value);

        if (input.AssignedUserId.HasValue)
            query = query.Where(r => r.AssignedUserId == input.AssignedUserId.Value);

        if (input.Status.HasValue)
            query = query.Where(r => r.Status == input.Status.Value);

        return query;
    }
}
