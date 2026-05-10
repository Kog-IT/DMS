using Abp.Application.Services.Dto;
using DMS.SalesmanRequests;
using DMS.SalesmanRequests.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.SalesmanRequests;

public class SalesmanRequestAppService_Tests : DMSTestBase
{
    private readonly ISalesmanRequestAppService _salesmanRequestAppService;

    public SalesmanRequestAppService_Tests()
    {
        _salesmanRequestAppService = Resolve<ISalesmanRequestAppService>();
    }

    [Fact]
    public async Task CreateSalesmanRequest_ShouldPersist()
    {
        var result = await _salesmanRequestAppService.CreateAsync(new CreateSalesmanRequestDto
        {
            SalesmanId = 1,
            WarehouseId = 1,
            RequestDate = new DateTime(2026, 1, 15),
            Notes = "Test salesman request",
            Items = new List<CreateSalesmanRequestItemDto>
            {
                new CreateSalesmanRequestItemDto { ProductId = 1, Quantity = 10 }
            }
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.SalesmanId.ShouldBe(1);

        await UsingDbContextAsync(async context =>
        {
            var request = await context.Set<SalesmanRequest>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            request.ShouldNotBeNull();
            request.SalesmanId.ShouldBe(1);
            request.WarehouseId.ShouldBe(1);

            var itemCount = await context.Set<SalesmanRequestItem>()
                .CountAsync(x => x.RequestId == result.Data.Id);
            itemCount.ShouldBe(1);
        });
    }

    [Fact]
    public async Task ApproveSalesmanRequest_ShouldSetStatusApproved()
    {
        var result = await _salesmanRequestAppService.CreateAsync(new CreateSalesmanRequestDto
        {
            SalesmanId = 1,
            WarehouseId = 1,
            RequestDate = new DateTime(2026, 1, 20),
            Notes = "Request to approve"
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        await _salesmanRequestAppService.ApproveAsync(new EntityDto<int> { Id = result.Data.Id });

        await UsingDbContextAsync(async context =>
        {
            var request = await context.Set<SalesmanRequest>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            request.ShouldNotBeNull();
            request.Status.ShouldBe(1);
        });
    }
}
