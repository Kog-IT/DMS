using Abp.Application.Services.Dto;
using DMS.Dispatches;
using DMS.Dispatches.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Dispatches;

public class DispatchAppService_Tests : DMSTestBase
{
    private readonly IPlannedDispatchAppService _plannedDispatchAppService;
    private readonly IActualDispatchAppService _actualDispatchAppService;

    public DispatchAppService_Tests()
    {
        _plannedDispatchAppService = Resolve<IPlannedDispatchAppService>();
        _actualDispatchAppService = Resolve<IActualDispatchAppService>();
    }

    [Fact]
    public async Task CreatePlannedDispatch_ShouldPersist()
    {
        var dispatchDate = new DateTime(2026, 1, 15);

        var result = await _plannedDispatchAppService.CreateAsync(new CreatePlannedDispatchDto
        {
            SalesmanId = 1,
            DispatchDate = dispatchDate,
            Notes = "Test planned dispatch",
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.SalesmanId.ShouldBe(1);

        await UsingDbContextAsync(async context =>
        {
            var dispatch = await context.Set<PlannedDispatch>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            dispatch.ShouldNotBeNull();
            dispatch.SalesmanId.ShouldBe(1);
            dispatch.DispatchDate.ShouldBe(dispatchDate);
            dispatch.IsActive.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task CreateActualDispatch_ShouldPersist()
    {
        var dispatchDate = new DateTime(2026, 1, 20);

        var result = await _actualDispatchAppService.CreateAsync(new CreateActualDispatchDto
        {
            SalesmanId = 1,
            DispatchDate = dispatchDate,
            Notes = "Test actual dispatch"
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.SalesmanId.ShouldBe(1);

        await UsingDbContextAsync(async context =>
        {
            var dispatch = await context.Set<ActualDispatch>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            dispatch.ShouldNotBeNull();
            dispatch.SalesmanId.ShouldBe(1);
        });
    }
}
