using Abp.Application.Services.Dto;
using DMS.ProductGroups;
using DMS.ProductGroups.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.ProductGroups;

public class ProductGroupAppService_Tests : DMSTestBase
{
    private readonly IProductGroupAppService _productGroupAppService;

    public ProductGroupAppService_Tests()
    {
        _productGroupAppService = Resolve<IProductGroupAppService>();
    }

    [Fact]
    public async Task CreateProductGroup_ShouldPersist()
    {
        var result = await _productGroupAppService.CreateAsync(new CreateProductGroupDto
        {
            Name = "Test ProductGroup",
            Name_EN = "Test ProductGroup EN",
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Test ProductGroup");
        result.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task DeactivateProductGroup_ShouldSetIsActiveFalse()
    {
        var created = await _productGroupAppService.CreateAsync(new CreateProductGroupDto
        {
            Name = "Active ProductGroup",
            IsActive = true
        });

        var result = await _productGroupAppService.DeactivateAsync(
            new EntityDto<int>(created.Data.Id));

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var entity = await context.Set<ProductGroup>().FirstOrDefaultAsync(x => x.Id == created.Data.Id);
            entity.IsActive.ShouldBeFalse();
        });
    }

    [Fact]
    public async Task BulkActivateProductGroups_ShouldSetIsActiveTrue()
    {
        var pg1 = await _productGroupAppService.CreateAsync(new CreateProductGroupDto { Name = "BulkAct1", IsActive = false });
        var pg2 = await _productGroupAppService.CreateAsync(new CreateProductGroupDto { Name = "BulkAct2", IsActive = false });

        var result = await _productGroupAppService.BulkActivateAsync(
            new List<int> { pg1.Data.Id, pg2.Data.Id });

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var entities = await context.Set<ProductGroup>()
                .IgnoreQueryFilters()
                .Where(x => x.Id == pg1.Data.Id || x.Id == pg2.Data.Id)
                .ToListAsync();
            entities.ShouldAllBe(x => x.IsActive);
        });
    }
}
