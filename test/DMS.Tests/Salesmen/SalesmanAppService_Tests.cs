using Abp.Application.Services.Dto;
using DMS.Salesmen;
using DMS.Salesmen.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Salesmen;

public class SalesmanAppService_Tests : DMSTestBase
{
    private readonly ISalesmanAppService _salesmanAppService;

    public SalesmanAppService_Tests()
    {
        _salesmanAppService = Resolve<ISalesmanAppService>();
    }

    [Fact]
    public async Task CreateSalesman_ShouldPersist()
    {
        var result = await _salesmanAppService.CreateAsync(new CreateSalesmanDto
        {
            Name = "Test Salesman",
            JobCode = "SAL001",
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Test Salesman");
        result.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task BulkDeleteSalesmen_ShouldSoftDelete()
    {
        var s1 = await _salesmanAppService.CreateAsync(new CreateSalesmanDto { Name = "BulkDel1" });
        var s2 = await _salesmanAppService.CreateAsync(new CreateSalesmanDto { Name = "BulkDel2" });

        var result = await _salesmanAppService.BulkDeleteAsync(
            new List<int> { s1.Data.Id, s2.Data.Id });

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var salesmen = await context.Set<Salesman>()
                .IgnoreQueryFilters()
                .Where(x => x.Id == s1.Data.Id || x.Id == s2.Data.Id)
                .ToListAsync();
            salesmen.ShouldAllBe(x => x.IsDeleted);
        });
    }

    [Fact]
    public async Task GetSelectList_ShouldReturnActiveOnly()
    {
        var active = await _salesmanAppService.CreateAsync(new CreateSalesmanDto
        {
            Name = "Active Salesman",
            IsActive = true
        });

        var inactive = await _salesmanAppService.CreateAsync(new CreateSalesmanDto
        {
            Name = "Inactive Salesman",
            IsActive = false
        });

        var result = await _salesmanAppService.GetSelectListAsync();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Any(x => x.Id == active.Data.Id).ShouldBeTrue();
        result.Data.Any(x => x.Id == inactive.Data.Id).ShouldBeFalse();
    }
}
