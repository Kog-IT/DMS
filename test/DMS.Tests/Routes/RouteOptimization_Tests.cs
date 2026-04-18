using DMS.Customers;
using DMS.Customers.Dto;
using DMS.Routes;
using DMS.Routes.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Routes;

public class RouteOptimization_Tests : DMSTestBase
{
    private readonly IRouteAppService _routeAppService;
    private readonly ICustomerAppService _customerAppService;

    public RouteOptimization_Tests()
    {
        _routeAppService = Resolve<IRouteAppService>();
        _customerAppService = Resolve<ICustomerAppService>();
    }

    private async Task<int> CreateCustomerWithGps(string code, double lat, double lon)
    {
        var dto = await _customerAppService.CreateAsync(new CreateCustomerDto
        {
            Code = code,
            Name = $"Customer {code}",
            Latitude = lat,
            Longitude = lon,
            IsActive = true
        });
        return dto.Id;
    }

    private async Task<int> CreateCustomerNoGps(string code)
    {
        var dto = await _customerAppService.CreateAsync(new CreateCustomerDto
        {
            Code = code,
            Name = $"Customer {code}",
            IsActive = true
        });
        return dto.Id;
    }

    private async Task<RouteDto> CreateRouteWithItems(List<CreateRouteItemDto> items)
    {
        return await _routeAppService.CreateAsync(new CreateRouteDto
        {
            Name = "Test Route",
            AssignedUserId = AbpSession.UserId.Value,
            PlannedDate = DateTime.Today.AddDays(1),
            Items = items
        });
    }

    private OptimizeRouteInputDto RepAt(int routeId, double lat, double lon) => new OptimizeRouteInputDto
    {
        RouteId = routeId,
        RepLatitude = lat,
        RepLongitude = lon,
        StartTime = new DateTime(2026, 4, 15, 8, 0, 0)
    };

    [Fact]
    public async Task Optimize_Empty_Route_Returns_Empty_Result()
    {
        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>());

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.RouteId.ShouldBe(route.Id);
        result.Items.ShouldBeEmpty();
        result.TotalDistanceKm.ShouldBe(0);
        result.TotalDurationMinutes.ShouldBe(0);
    }

    [Fact]
    public async Task Optimize_Single_Stop_Returns_Correct_Order()
    {
        var custId = await CreateCustomerWithGps("C001", 30.05, 31.25);
        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = custId, OrderIndex = 0 }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.Items.Count.ShouldBe(1);
        result.Items[0].CustomerId.ShouldBe(custId);
        result.Items[0].OrderIndex.ShouldBe(0);
        result.Items[0].DistanceFromPreviousKm.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Optimize_Sorts_By_Proximity_From_Rep_Location()
    {
        // Rep is at (30.0, 31.0)
        // Near customer at (30.01, 31.01) — closer
        // Far customer at (30.10, 31.10) — farther
        var nearId = await CreateCustomerWithGps("NEAR", 30.01, 31.01);
        var farId = await CreateCustomerWithGps("FAR", 30.10, 31.10);

        // Far is first in original order
        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = farId, OrderIndex = 0 },
            new CreateRouteItemDto { CustomerId = nearId, OrderIndex = 1 }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.Items.Count.ShouldBe(2);
        result.Items.First(i => i.CustomerId == nearId).OrderIndex.ShouldBe(0);
        result.Items.First(i => i.CustomerId == farId).OrderIndex.ShouldBe(1);
    }

    [Fact]
    public async Task Optimize_Skips_Customers_Without_GPS_And_Places_At_End()
    {
        var gpsId = await CreateCustomerWithGps("GPS", 30.05, 31.05);
        var noGpsId = await CreateCustomerNoGps("NOGPS");

        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = noGpsId, OrderIndex = 0 },
            new CreateRouteItemDto { CustomerId = gpsId, OrderIndex = 1 }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.Items.Count.ShouldBe(2);
        result.Items.First(i => i.CustomerId == gpsId).OrderIndex.ShouldBe(0);
        result.Items.First(i => i.CustomerId == noGpsId).OrderIndex.ShouldBe(1);
    }

    [Fact]
    public async Task Optimize_Calculates_Duration_Using_PlannedDurationMinutes()
    {
        var custId = await CreateCustomerWithGps("C001", 30.05, 31.05);
        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = custId, OrderIndex = 0, PlannedDurationMinutes = 45 }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.Items[0].PlannedDurationMinutes.ShouldBe(45);
    }

    [Fact]
    public async Task Optimize_Falls_Back_To_Default_Duration_When_PlannedDuration_Is_Null()
    {
        var custId = await CreateCustomerWithGps("C001", 30.05, 31.05);
        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = custId, OrderIndex = 0, PlannedDurationMinutes = null }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.Items[0].PlannedDurationMinutes.ShouldBe(30);
    }

    [Fact]
    public async Task Optimize_Updates_RouteItem_OrderIndex_In_Database()
    {
        var nearId = await CreateCustomerWithGps("NEAR", 30.01, 31.01);
        var farId = await CreateCustomerWithGps("FAR", 30.10, 31.10);

        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = farId, OrderIndex = 0 },
            new CreateRouteItemDto { CustomerId = nearId, OrderIndex = 1 }
        });

        await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        await UsingDbContextAsync(async ctx =>
        {
            var items = await ctx.Set<RouteItem>()
                .Where(i => i.RouteId == route.Id)
                .ToListAsync();

            var nearItem = items.FirstOrDefault(i => i.CustomerId == nearId);
            var farItem = items.FirstOrDefault(i => i.CustomerId == farId);

            nearItem.ShouldNotBeNull();
            farItem.ShouldNotBeNull();
            nearItem.OrderIndex.ShouldBe(0);
            farItem.OrderIndex.ShouldBe(1);
        });
    }

    [Fact]
    public async Task Optimize_Returns_TotalDurationMinutes()
    {
        var c1 = await CreateCustomerWithGps("C1", 30.01, 31.01);
        var c2 = await CreateCustomerWithGps("C2", 30.02, 31.02);

        var route = await CreateRouteWithItems(new List<CreateRouteItemDto>
        {
            new CreateRouteItemDto { CustomerId = c1, OrderIndex = 0, PlannedDurationMinutes = 20 },
            new CreateRouteItemDto { CustomerId = c2, OrderIndex = 1, PlannedDurationMinutes = 30 }
        });

        var result = await _routeAppService.OptimizeRouteAsync(RepAt(route.Id, 30.0, 31.0));

        result.TotalDurationMinutes.ShouldBeGreaterThanOrEqualTo(50);
        result.TotalDistanceKm.ShouldBeGreaterThan(0);
        result.EstimatedEndTime.ShouldBeGreaterThan(new DateTime(2026, 4, 15, 8, 0, 0));
    }
}
