using Abp.Domain.Repositories;
using DMS.Common.Dto;
using DMS.Visits.Dto.Analytics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Visits;

public class VisitAnalyticsAppService : DMSAppServiceBase, IVisitAnalyticsAppService
{
    private readonly IRepository<Visit, int> _visitRepository;

    public VisitAnalyticsAppService(IRepository<Visit, int> visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<ApiResponse<VisitCoverageDto>> GetCoverageAsync(VisitAnalyticsFilterDto input)
    {
        var query = BuildQuery(input);
        var visits = await query.ToListAsync();
        int total = visits.Count;

        var dto = new VisitCoverageDto
        {
            TotalPlanned = total,
            Completed  = visits.Count(v => v.Status == VisitStatus.Completed),
            Skipped    = visits.Count(v => v.Status == VisitStatus.Skipped),
            NoSale     = 0,
            InProgress = visits.Count(v => v.Status == VisitStatus.InProgress),
        };

        dto.CoveragePercentage = total == 0 ? 0 : Math.Round((double)dto.Completed / total * 100, 2);
        dto.CompletionRate     = total == 0 ? 0 : Math.Round((double)dto.Completed / total * 100, 2);

        return Ok(dto, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<VisitDurationStatsDto>> GetDurationStatsAsync(VisitAnalyticsFilterDto input)
    {
        var query = BuildQuery(input).Where(v => v.DurationMinutes.HasValue && v.DurationMinutes > 0);
        var durations = await query.Select(v => v.DurationMinutes!.Value).ToListAsync();

        if (!durations.Any())
            return Ok(new VisitDurationStatsDto(), L("SuccessfullyRetrieved"));

        var dto = new VisitDurationStatsDto
        {
            TotalVisitsWithDuration = durations.Count,
            AverageDurationMinutes  = Math.Round(durations.Average(), 2),
            MinDurationMinutes      = durations.Min(),
            MaxDurationMinutes      = durations.Max(),
        };

        return Ok(dto, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<List<VisitStatusCountDto>>> GetStatusChartAsync(VisitAnalyticsFilterDto input)
    {
        var query = BuildQuery(input);
        var visits = await query.ToListAsync();
        int total = visits.Count;

        var groups = visits
            .GroupBy(v => v.Status)
            .Select(g => new VisitStatusCountDto
            {
                StatusName = g.Key.ToString(),
                Count      = g.Count(),
                Percentage = total == 0 ? 0 : Math.Round((double)g.Count() / total * 100, 2),
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Ok(groups, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<List<VisitDailyStatDto>>> GetDailyStatsAsync(VisitAnalyticsFilterDto input)
    {
        var query = BuildQuery(input);
        var visits = await query.ToListAsync();

        var daily = visits
            .GroupBy(v => v.PlannedDate.Date)
            .Select(g => new VisitDailyStatDto
            {
                Date      = g.Key,
                Planned   = g.Count(),
                Completed = g.Count(v => v.Status == VisitStatus.Completed),
                Skipped   = g.Count(v => v.Status == VisitStatus.Skipped),
            })
            .OrderBy(x => x.Date)
            .ToList();

        return Ok(daily, L("SuccessfullyRetrieved"));
    }

    private IQueryable<Visit> BuildQuery(VisitAnalyticsFilterDto input)
    {
        var query = _visitRepository.GetAll();

        if (input.DateFrom.HasValue)
            query = query.Where(v => v.PlannedDate >= input.DateFrom.Value);

        if (input.DateTo.HasValue)
            query = query.Where(v => v.PlannedDate <= input.DateTo.Value);

        if (input.AssignedUserId.HasValue)
            query = query.Where(v => v.AssignedUserId == input.AssignedUserId.Value);

        if (input.RouteId.HasValue)
            query = query.Where(v => v.RouteId == input.RouteId.Value);

        return query;
    }
}
