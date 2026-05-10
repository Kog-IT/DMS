using DMS.Visits;
using DMS.Visits.Dto.Analytics;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Visits;

public class VisitAnalyticsAppService_Tests : DMSTestBase
{
    private readonly IVisitAnalyticsAppService _service;

    public VisitAnalyticsAppService_Tests()
    {
        _service = Resolve<IVisitAnalyticsAppService>();
    }

    [Fact]
    public async Task GetCoverage_WithNoVisits_ReturnsZero()
    {
        var result = await _service.GetCoverageAsync(new VisitAnalyticsFilterDto
        {
            DateFrom = new DateTime(2099, 1, 1),
            DateTo   = new DateTime(2099, 1, 31),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.TotalPlanned.ShouldBe(0);
        result.Data.CoveragePercentage.ShouldBe(0);
    }

    [Fact]
    public async Task GetStatusChart_ReturnsGroupedByStatus()
    {
        await UsingDbContextAsync(async context =>
        {
            context.Set<Visit>().Add(new Visit
            {
                TenantId       = 1,
                CustomerId     = 1,
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = VisitStatus.Completed,
            });
            context.Set<Visit>().Add(new Visit
            {
                TenantId       = 1,
                CustomerId     = 1,
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = VisitStatus.Completed,
            });
            await Task.CompletedTask;
        });

        var result = await _service.GetStatusChartAsync(new VisitAnalyticsFilterDto
        {
            DateFrom = DateTime.Today.AddDays(-1),
            DateTo   = DateTime.Today.AddDays(1),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(x => x.StatusName == "Completed" && x.Count >= 2);
    }

    [Fact]
    public async Task GetDurationStats_ReturnsCorrectAverage()
    {
        await UsingDbContextAsync(async context =>
        {
            context.Set<Visit>().Add(new Visit
            {
                TenantId        = 1,
                CustomerId      = 1,
                AssignedUserId  = 1,
                PlannedDate     = DateTime.Today,
                Status          = VisitStatus.Completed,
                DurationMinutes = 30,
            });
            context.Set<Visit>().Add(new Visit
            {
                TenantId        = 1,
                CustomerId      = 1,
                AssignedUserId  = 1,
                PlannedDate     = DateTime.Today,
                Status          = VisitStatus.Completed,
                DurationMinutes = 60,
            });
            await Task.CompletedTask;
        });

        var result = await _service.GetDurationStatsAsync(new VisitAnalyticsFilterDto
        {
            DateFrom = DateTime.Today.AddDays(-1),
            DateTo   = DateTime.Today.AddDays(1),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.AverageDurationMinutes.ShouldBe(45);
    }
}
