using Abp.Application.Services.Dto;
using DMS.Brands;
using DMS.Brands.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Brands;

public class BrandAppService_Tests : DMSTestBase
{
    private readonly IBrandAppService _brandAppService;

    public BrandAppService_Tests()
    {
        _brandAppService = Resolve<IBrandAppService>();
    }

    [Fact]
    public async Task CreateBrand_ShouldPersist()
    {
        var result = await _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = "Test Brand",
            Name_EN = "Test Brand EN",
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Test Brand");
        result.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task ActivateBrand_ShouldSetIsActiveTrue()
    {
        var created = await _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = "Inactive Brand",
            IsActive = false
        });

        var result = await _brandAppService.ActivateAsync(
            new EntityDto<int>(created.Data.Id));

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var brand = await context.Set<Brand>().FirstOrDefaultAsync(b => b.Id == created.Data.Id);
            brand.IsActive.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task BulkDeleteBrands_ShouldRemoveAll()
    {
        var b1 = await _brandAppService.CreateAsync(new CreateBrandDto { Name = "BulkDel1" });
        var b2 = await _brandAppService.CreateAsync(new CreateBrandDto { Name = "BulkDel2" });

        var result = await _brandAppService.BulkDeleteAsync(
            new List<int> { b1.Data.Id, b2.Data.Id });

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var brands = await context.Set<Brand>()
                .IgnoreQueryFilters()
                .Where(b => b.Id == b1.Data.Id || b.Id == b2.Data.Id)
                .ToListAsync();
            brands.ShouldAllBe(b => b.IsDeleted);
        });
    }
}
