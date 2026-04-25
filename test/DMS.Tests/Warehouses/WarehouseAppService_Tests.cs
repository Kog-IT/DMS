using Abp.Application.Services.Dto;
using DMS.Warehouses;
using DMS.Warehouses.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Warehouses;

public class WarehouseAppService_Tests : DMSTestBase
{
    private readonly IWarehouseAppService _warehouseAppService;

    public WarehouseAppService_Tests()
    {
        _warehouseAppService = Resolve<IWarehouseAppService>();
    }

    [Fact]
    public async Task CreateWarehouse_ShouldPersist()
    {
        var result = await _warehouseAppService.CreateAsync(new CreateWarehouseDto
        {
            Name = "Main Warehouse",
            Code = "WH-001",
            GovernorateId = 1,
            CityId = 1,
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Main Warehouse");
        result.Data.Code.ShouldBe("WH-001");
        result.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task BulkDeleteWarehouses_ShouldSoftDelete()
    {
        var w1 = await _warehouseAppService.CreateAsync(new CreateWarehouseDto
        {
            Name = "BulkDel1", Code = "WH-BD1", GovernorateId = 1, CityId = 1
        });
        var w2 = await _warehouseAppService.CreateAsync(new CreateWarehouseDto
        {
            Name = "BulkDel2", Code = "WH-BD2", GovernorateId = 1, CityId = 1
        });

        await _warehouseAppService.BulkDeleteAsync(
            new List<int> { w1.Data.Id, w2.Data.Id });

        await UsingDbContextAsync(async context =>
        {
            var warehouses = await context.Set<Warehouse>()
                .IgnoreQueryFilters()
                .Where(w => w.Id == w1.Data.Id || w.Id == w2.Data.Id)
                .ToListAsync();
            warehouses.ShouldAllBe(w => w.IsDeleted);
        });
    }

    [Fact]
    public async Task AssignProducts_ShouldReplaceExisting()
    {
        var warehouse = await _warehouseAppService.CreateAsync(new CreateWarehouseDto
        {
            Name = "Product Warehouse", Code = "WH-PD1", GovernorateId = 1, CityId = 1
        });

        await _warehouseAppService.AssignProductsAsync(new WarehouseProductCreateDto
        {
            WarehouseId = warehouse.Data.Id,
            ProductWarehouses = new List<ProductItemDto>
            {
                new ProductItemDto { ProductId = 1, Quantity = 10, WeightPerKG = 2.5m }
            }
        });

        await _warehouseAppService.AssignProductsAsync(new WarehouseProductCreateDto
        {
            WarehouseId = warehouse.Data.Id,
            ProductWarehouses = new List<ProductItemDto>
            {
                new ProductItemDto { ProductId = 2, Quantity = 5, WeightPerKG = 1.0m }
            }
        });

        await UsingDbContextAsync(async context =>
        {
            var products = await context.Set<WarehouseProduct>()
                .Where(wp => wp.WarehouseId == warehouse.Data.Id)
                .ToListAsync();
            products.Count.ShouldBe(1);
            products[0].ProductId.ShouldBe(2);
        });
    }
}
