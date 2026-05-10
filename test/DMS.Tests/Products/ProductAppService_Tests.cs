using Abp.Application.Services.Dto;
using DMS.Categories;
using DMS.Categories.Dto;
using DMS.Products;
using DMS.Products.Dto;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Products;

public class ProductAppService_Tests : DMSTestBase
{
    private readonly IProductAppService _productAppService;
    private readonly ICategoryAppService _categoryAppService;

    public ProductAppService_Tests()
    {
        _productAppService = Resolve<IProductAppService>();
        _categoryAppService = Resolve<ICategoryAppService>();
    }

    private async Task<int> CreateCategoryAsync(string name = "Beverages")
    {
        var cat = await _categoryAppService.CreateAsync(new CreateCategoryDto { Name = name });
        return cat.Data.Id;
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        var result = await _productAppService.GetAllAsync(
            new PagedProductResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.TotalCount.ShouldBe(0);
        result.Data.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Create_And_Get_Product()
    {
        var catId = await CreateCategoryAsync();

        var created = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = "Cola 330ml",
            Description = "Carbonated drink",
            Price = 5.50m,
            CategoryId = catId
        });

        created.Data.Id.ShouldBeGreaterThan(0);
        created.Data.Name.ShouldBe("Cola 330ml");
        created.Data.Price.ShouldBe(5.50m);
        created.Data.CategoryId.ShouldBe(catId);

        var fetched = await _productAppService.GetAsync(new EntityDto<int>(created.Data.Id));
        fetched.Data.Name.ShouldBe("Cola 330ml");
        fetched.Data.CategoryName.ShouldBe("Beverages");
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword()
    {
        var catId = await CreateCategoryAsync();
        await _productAppService.CreateAsync(new CreateProductDto { Name = "Cola 330ml", Price = 5m, CategoryId = catId });
        await _productAppService.CreateAsync(new CreateProductDto { Name = "Water 500ml", Price = 2m, CategoryId = catId });

        var result = await _productAppService.GetAllAsync(
            new PagedProductResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "Cola" });

        result.Data.TotalCount.ShouldBe(1);
        result.Data.Items[0].Name.ShouldBe("Cola 330ml");
    }

    [Fact]
    public async Task GetAll_Filters_By_CategoryId()
    {
        var catA = await CreateCategoryAsync("Beverages");
        var catB = await CreateCategoryAsync("Snacks");

        await _productAppService.CreateAsync(new CreateProductDto { Name = "Cola", Price = 5m, CategoryId = catA });
        await _productAppService.CreateAsync(new CreateProductDto { Name = "Chips", Price = 3m, CategoryId = catB });

        var result = await _productAppService.GetAllAsync(
            new PagedProductResultRequestDto { MaxResultCount = 20, SkipCount = 0, CategoryId = catB });

        result.Data.TotalCount.ShouldBe(1);
        result.Data.Items[0].Name.ShouldBe("Chips");
    }

    [Fact]
    public async Task Update_Product()
    {
        var catId = await CreateCategoryAsync();
        var created = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = "Old Name",
            Price = 1m,
            CategoryId = catId
        });

        var updated = await _productAppService.UpdateAsync(new UpdateProductDto
        {
            Id = created.Data.Id,
            Name = "New Name",
            Price = 9.99m,
            CategoryId = catId
        });

        updated.Data.Name.ShouldBe("New Name");
        updated.Data.Price.ShouldBe(9.99m);
    }

    [Fact]
    public async Task Delete_Product()
    {
        var catId = await CreateCategoryAsync();
        var created = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = "To Delete",
            Price = 1m,
            CategoryId = catId
        });

        await _productAppService.DeleteAsync(new EntityDto<int>(created.Data.Id));

        var result = await _productAppService.GetAllAsync(
            new PagedProductResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.Items.ShouldNotContain(p => p.Id == created.Data.Id);
    }
}
