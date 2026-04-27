using Abp.Application.Services.Dto;
using DMS.CustomerGroups;
using DMS.CustomerGroups.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.CustomerGroups;

public class CustomerGroupAppService_Tests : DMSTestBase
{
    private readonly ICustomerGroupAppService _customerGroupAppService;

    public CustomerGroupAppService_Tests()
    {
        _customerGroupAppService = Resolve<ICustomerGroupAppService>();
    }

    [Fact]
    public async Task CreateCustomerGroup_ShouldPersist()
    {
        var result = await _customerGroupAppService.CreateAsync(new CreateCustomerGroupDto
        {
            Name = "Test Group",
            IsTaxExempted = false,
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Test Group");
        result.Data.IsActive.ShouldBeTrue();
        result.Data.IsTaxExempted.ShouldBeFalse();
    }

    [Fact]
    public async Task BulkDeleteCustomerGroups_ShouldSoftDelete()
    {
        var g1 = await _customerGroupAppService.CreateAsync(new CreateCustomerGroupDto { Name = "BulkDel1" });
        var g2 = await _customerGroupAppService.CreateAsync(new CreateCustomerGroupDto { Name = "BulkDel2" });

        var result = await _customerGroupAppService.BulkDeleteAsync(
            new List<int> { g1.Data.Id, g2.Data.Id });

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var groups = await context.Set<CustomerGroup>()
                .IgnoreQueryFilters()
                .Where(x => x.Id == g1.Data.Id || x.Id == g2.Data.Id)
                .ToListAsync();
            groups.ShouldAllBe(x => x.IsDeleted);
        });
    }

    [Fact]
    public async Task UpdateTaxExempted_ShouldUpdate()
    {
        var created = await _customerGroupAppService.CreateAsync(new CreateCustomerGroupDto
        {
            Name = "Tax Group",
            IsTaxExempted = false
        });

        await _customerGroupAppService.UpdateTaxExemptedAsync(new UpdateCustomerGroupTaxExemptedDto { Id = created.Data.Id, IsTaxExempted = true });

        await UsingDbContextAsync(async context =>
        {
            var group = await context.Set<CustomerGroup>()
                .FirstOrDefaultAsync(x => x.Id == created.Data.Id);
            group.IsTaxExempted.ShouldBeTrue();
        });
    }
}
