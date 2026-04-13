using Abp.Application.Services.Dto;
using DMS.Routes;
using DMS.Routes.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Routes;

public class RouteAppService_Tests : DMSTestBase
{
    private readonly IRouteAppService _routeAppService;

    public RouteAppService_Tests()
    {
        _routeAppService = Resolve<IRouteAppService>();
    }

    private CreateRouteDto SampleRoute(string name = "Cairo North") => new CreateRouteDto
    {
        Name = name,
        AssignedUserId = AbpSession.UserId.Value,
        PlannedDate = DateTime.Today.AddDays(1)
    };

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        var result = await _routeAppService.GetAllAsync(
            new PagedRouteResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.TotalCount.ShouldBe(0);
        result.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Create_And_Get_Route()
    {
        var created = await _routeAppService.CreateAsync(SampleRoute("Cairo North"));

        created.Id.ShouldBeGreaterThan(0);
        created.Name.ShouldBe("Cairo North");
        created.Status.ShouldBe(RouteStatus.Draft);
        created.AssignedUserId.ShouldBe(AbpSession.UserId.Value);
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword()
    {
        await _routeAppService.CreateAsync(SampleRoute("Cairo North"));
        await _routeAppService.CreateAsync(SampleRoute("Alex South"));

        var result = await _routeAppService.GetAllAsync(
            new PagedRouteResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "Cairo" });

        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("Cairo North");
    }

    [Fact]
    public async Task GetAll_Filters_By_AssignedUser()
    {
        await _routeAppService.CreateAsync(SampleRoute("Route A"));

        var result = await _routeAppService.GetAllAsync(
            new PagedRouteResultRequestDto
            {
                MaxResultCount = 20,
                SkipCount = 0,
                AssignedUserId = AbpSession.UserId.Value
            });

        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("Route A");
    }

    [Fact]
    public async Task Update_Route()
    {
        var created = await _routeAppService.CreateAsync(SampleRoute("Old Name"));

        var updated = await _routeAppService.UpdateAsync(new UpdateRouteDto
        {
            Id = created.Id,
            Name = "New Name",
            AssignedUserId = AbpSession.UserId.Value,
            PlannedDate = created.PlannedDate,
            Status = RouteStatus.Active
        });

        updated.Name.ShouldBe("New Name");
        updated.Status.ShouldBe(RouteStatus.Active);
    }

    [Fact]
    public async Task Delete_Route()
    {
        var created = await _routeAppService.CreateAsync(SampleRoute());

        await _routeAppService.DeleteAsync(new EntityDto<int>(created.Id));

        var result = await _routeAppService.GetAllAsync(
            new PagedRouteResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Items.ShouldNotContain(r => r.Id == created.Id);
    }
}
