using Abp.Application.Services.Dto;
using DMS.Categories;
using DMS.Categories.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Categories;

public class CategoryAppService_Tests : DMSTestBase
{
    private readonly ICategoryAppService _categoryAppService;

    public CategoryAppService_Tests()
    {
        _categoryAppService = Resolve<ICategoryAppService>();
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        var result = await _categoryAppService.GetAllAsync(
            new PagedCategoryResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.TotalCount.ShouldBe(0);
        result.Data.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Create_And_Get_Category()
    {
        var created = await _categoryAppService.CreateAsync(
            new CreateCategoryDto { Name = "Beverages" });

        created.Data.Id.ShouldBeGreaterThan(0);
        created.Data.Name.ShouldBe("Beverages");

        var fetched = await _categoryAppService.GetAsync(new EntityDto<int>(created.Data.Id));
        fetched.Data.Name.ShouldBe("Beverages");
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword()
    {
        await _categoryAppService.CreateAsync(new CreateCategoryDto { Name = "Beverages" });
        await _categoryAppService.CreateAsync(new CreateCategoryDto { Name = "Snacks" });

        var result = await _categoryAppService.GetAllAsync(
            new PagedCategoryResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "Bev" });

        result.Data.TotalCount.ShouldBe(1);
        result.Data.Items[0].Name.ShouldBe("Beverages");
    }

    [Fact]
    public async Task Update_Category()
    {
        var created = await _categoryAppService.CreateAsync(
            new CreateCategoryDto { Name = "Old Name" });

        var updated = await _categoryAppService.UpdateAsync(new UpdateCategoryDto
        {
            Id = created.Data.Id,
            Name = "New Name"
        });

        updated.Data.Name.ShouldBe("New Name");
    }

    [Fact]
    public async Task Delete_Category()
    {
        var created = await _categoryAppService.CreateAsync(
            new CreateCategoryDto { Name = "To Delete" });

        await _categoryAppService.DeleteAsync(new EntityDto<int>(created.Data.Id));

        var result = await _categoryAppService.GetAllAsync(
            new PagedCategoryResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.Items.ShouldNotContain(c => c.Id == created.Data.Id);
    }

    [Fact]
    public async Task Tenant_Isolation_TenantId_Stamped_Correctly()
    {
        var created = await _categoryAppService.CreateAsync(
            new CreateCategoryDto { Name = "Tenant Category" });

        await UsingDbContextAsync(async context =>
        {
            var category = await context.Set<Category>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == created.Data.Id);

            category.ShouldNotBeNull();
            category.TenantId.ShouldBe(AbpSession.TenantId.Value);
        });
    }
}
