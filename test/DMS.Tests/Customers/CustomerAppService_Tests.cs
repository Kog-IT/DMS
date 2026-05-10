using Abp.Application.Services.Dto;
using DMS.Customers;
using DMS.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Customers;

public class CustomerAppService_Tests : DMSTestBase
{
    private readonly ICustomerAppService _customerAppService;

    public CustomerAppService_Tests()
    {
        _customerAppService = Resolve<ICustomerAppService>();
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        var result = await _customerAppService.GetAllAsync(
            new PagedCustomerResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.TotalCount.ShouldBe(0);
        result.Data.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Create_And_Get_Customer()
    {
        var input = new CreateCustomerDto
        {
            Code = "CUST-001",
            Name = "Acme Distribution",
            Address = "12 Tahrir Square",
            Latitude = 30.0444,
            Longitude = 31.2357,
            Phone = "02-12345678",
            Email = "info@acme.eg",
            IsActive = true
        };

        var created = await _customerAppService.CreateAsync(input);

        created.Data.Id.ShouldBeGreaterThan(0);
        created.Data.Code.ShouldBe("CUST-001");
        created.Data.Name.ShouldBe("Acme Distribution");
        created.Data.Latitude.ShouldBe(30.0444);
        created.Data.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword_Name()
    {
        await _customerAppService.CreateAsync(new CreateCustomerDto { Code = "C001", Name = "Acme Corp", IsActive = true });
        await _customerAppService.CreateAsync(new CreateCustomerDto { Code = "C002", Name = "Beta Ltd", IsActive = true });

        var result = await _customerAppService.GetAllAsync(
            new PagedCustomerResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "Acme" });

        result.Data.TotalCount.ShouldBe(1);
        result.Data.Items[0].Name.ShouldBe("Acme Corp");
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword_Code()
    {
        await _customerAppService.CreateAsync(new CreateCustomerDto { Code = "NORTH-01", Name = "North Store", IsActive = true });
        await _customerAppService.CreateAsync(new CreateCustomerDto { Code = "SOUTH-01", Name = "South Store", IsActive = true });

        var result = await _customerAppService.GetAllAsync(
            new PagedCustomerResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "NORTH" });

        result.Data.TotalCount.ShouldBe(1);
        result.Data.Items[0].Code.ShouldBe("NORTH-01");
    }

    [Fact]
    public async Task Update_Customer()
    {
        var created = await _customerAppService.CreateAsync(
            new CreateCustomerDto { Code = "C001", Name = "Old Name", IsActive = true });

        var updated = await _customerAppService.UpdateAsync(new UpdateCustomerDto
        {
            Id = created.Data.Id,
            Code = "C001-NEW",
            Name = "New Name",
            IsActive = false
        });

        updated.Data.Code.ShouldBe("C001-NEW");
        updated.Data.Name.ShouldBe("New Name");
        updated.Data.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Delete_Customer()
    {
        var created = await _customerAppService.CreateAsync(
            new CreateCustomerDto { Code = "DEL-01", Name = "To Delete", IsActive = true });

        await _customerAppService.DeleteAsync(new EntityDto<int>(created.Data.Id));

        var result = await _customerAppService.GetAllAsync(
            new PagedCustomerResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Data.Items.ShouldNotContain(c => c.Id == created.Data.Id);
    }

    [Fact]
    public async Task Tenant_Isolation_Customer_TenantId_Stamped_Correctly()
    {
        // Arrange — create a customer as tenant 1 (default, already logged in as TenantId=1)
        var created = await _customerAppService.CreateAsync(
            new CreateCustomerDto { Code = "T-001", Name = "Tenant1 Customer", IsActive = true });

        // Assert — verify directly in DB that TenantId is stamped and data is scoped
        await UsingDbContextAsync(async context =>
        {
            var customer = await context.Set<DMS.Customers.Customer>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == created.Data.Id);

            customer.ShouldNotBeNull();
            customer.TenantId.ShouldBe(AbpSession.TenantId.Value);

            // Verify a query scoped to a different TenantId returns nothing
            var otherTenantCustomers = await context.Set<DMS.Customers.Customer>()
                .IgnoreQueryFilters()
                .Where(c => c.TenantId == 999 && c.Id == created.Data.Id)
                .ToListAsync();

            otherTenantCustomers.ShouldBeEmpty();
        });
    }
}
