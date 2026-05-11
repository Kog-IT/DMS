using DMS.Routes;
using DMS.Routes.Dto.Analytics;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Routes;

public class RouteAnalyticsAppService_Tests : DMSTestBase
{
    private readonly IRouteAnalyticsAppService _service;

    public RouteAnalyticsAppService_Tests()
    {
        _service = Resolve<IRouteAnalyticsAppService>();
    }

    [Fact]
    public async Task GetSummary_WithNoRoutes_ReturnsZeros()
    {
        var result = await _service.GetSummaryAsync(new RouteAnalyticsFilterDto
        {
            DateFrom = new DateTime(2099, 1, 1),
            DateTo   = new DateTime(2099, 1, 31),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.TotalRoutes.ShouldBe(0);
    }

    [Fact]
    public async Task GetStatusChart_ReturnsGroupedByStatus()
    {
        await UsingDbContextAsync(async context =>
        {
            context.Set<Route>().Add(new Route
            {
                TenantId       = 1,
                Name           = "Test Route",
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = RouteStatus.Draft,
            });
            context.Set<Route>().Add(new Route
            {
                TenantId       = 1,
                Name           = "Test Route",
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = RouteStatus.Completed,
            });
            await Task.CompletedTask;
        });

        var result = await _service.GetStatusChartAsync(new RouteAnalyticsFilterDto
        {
            DateFrom = DateTime.Today.AddDays(-1),
            DateTo   = DateTime.Today.AddDays(1),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetDailyStats_GroupsByDate()
    {
        await UsingDbContextAsync(async context =>
        {
            context.Set<Route>().Add(new Route
            {
                TenantId       = 1,
                Name           = "Test Route",
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = RouteStatus.Draft,
            });
            context.Set<Route>().Add(new Route
            {
                TenantId       = 1,
                Name           = "Test Route",
                AssignedUserId = 1,
                PlannedDate    = DateTime.Today,
                Status         = RouteStatus.Draft,
            });
            await Task.CompletedTask;
        });

        var result = await _service.GetDailyStatsAsync(new RouteAnalyticsFilterDto
        {
            DateFrom = DateTime.Today.AddDays(-1),
            DateTo   = DateTime.Today.AddDays(1),
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
        result.Data[0].TotalRoutes.ShouldBeGreaterThanOrEqualTo(2);
    }
}
