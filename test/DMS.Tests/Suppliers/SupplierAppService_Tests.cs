using Abp.Application.Services.Dto;
using DMS.Suppliers;
using DMS.Suppliers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Suppliers;

public class SupplierAppService_Tests : DMSTestBase
{
    private readonly ISupplierAppService _supplierAppService;

    public SupplierAppService_Tests()
    {
        _supplierAppService = Resolve<ISupplierAppService>();
    }

    [Fact]
    public async Task CreateSupplier_ShouldPersist()
    {
        var result = await _supplierAppService.CreateAsync(new CreateSupplierDto
        {
            Name = "Test Supplier",
            Code = "SUP001",
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Test Supplier");
        result.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task BulkDeleteSuppliers_ShouldSoftDelete()
    {
        var s1 = await _supplierAppService.CreateAsync(new CreateSupplierDto { Name = "BulkDel1" });
        var s2 = await _supplierAppService.CreateAsync(new CreateSupplierDto { Name = "BulkDel2" });

        var result = await _supplierAppService.BulkDeleteAsync(
            new List<int> { s1.Data.Id, s2.Data.Id });

        result.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var suppliers = await context.Set<Supplier>()
                .IgnoreQueryFilters()
                .Where(x => x.Id == s1.Data.Id || x.Id == s2.Data.Id)
                .ToListAsync();
            suppliers.ShouldAllBe(x => x.IsDeleted);
        });
    }

    [Fact]
    public async Task DeactivateSupplier_ShouldSetIsActiveFalse()
    {
        var created = await _supplierAppService.CreateAsync(new CreateSupplierDto
        {
            Name = "Active Supplier",
            IsActive = true
        });

        await _supplierAppService.DeactivateAsync(new EntityDto<int> { Id = created.Data.Id });

        await UsingDbContextAsync(async context =>
        {
            var supplier = await context.Set<Supplier>()
                .FirstOrDefaultAsync(x => x.Id == created.Data.Id);
            supplier.IsActive.ShouldBeFalse();
        });
    }
}
