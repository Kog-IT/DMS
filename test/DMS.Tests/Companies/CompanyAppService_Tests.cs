// test/DMS.Tests/Companies/CompanyAppService_Tests.cs
using Abp.Application.Services.Dto;
using DMS.Companies;
using DMS.Companies.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Companies;

public class CompanyAppService_Tests : DMSTestBase
{
    private readonly ICompanyAppService _companyAppService;

    public CompanyAppService_Tests()
    {
        _companyAppService = Resolve<ICompanyAppService>();
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        // Default tenant (id=1) has no companies seeded
        var result = await _companyAppService.GetAllAsync(
            new PagedCompanyResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.TotalCount.ShouldBe(0);
        result.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Create_And_Get_Company()
    {
        // Arrange
        var input = new CreateCompanyDto
        {
            Name = "Acme Corp",
            TaxNumber = "TAX123",
            Address = "123 Main St",
            Phone = "555-0100",
            Email = "contact@acme.com",
            IsActive = true
        };

        // Act
        var created = await _companyAppService.CreateAsync(input);

        // Assert
        created.Id.ShouldBeGreaterThan(0);
        created.Name.ShouldBe("Acme Corp");
        created.TaxNumber.ShouldBe("TAX123");
        created.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_Filters_By_Keyword()
    {
        // Arrange — create two companies
        await _companyAppService.CreateAsync(new CreateCompanyDto { Name = "Acme Corp", IsActive = true });
        await _companyAppService.CreateAsync(new CreateCompanyDto { Name = "Beta Ltd", IsActive = true });

        // Act
        var result = await _companyAppService.GetAllAsync(
            new PagedCompanyResultRequestDto { MaxResultCount = 20, SkipCount = 0, Keyword = "Acme" });

        // Assert
        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("Acme Corp");
    }

    [Fact]
    public async Task Update_Company()
    {
        // Arrange
        var created = await _companyAppService.CreateAsync(
            new CreateCompanyDto { Name = "Old Name", IsActive = true });

        // Act
        var updated = await _companyAppService.UpdateAsync(new UpdateCompanyDto
        {
            Id = created.Id,
            Name = "New Name",
            IsActive = false
        });

        // Assert
        updated.Name.ShouldBe("New Name");
        updated.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Delete_Company()
    {
        // Arrange
        var created = await _companyAppService.CreateAsync(
            new CreateCompanyDto { Name = "To Delete", IsActive = true });

        // Act
        await _companyAppService.DeleteAsync(new EntityDto<int>(created.Id));

        // Assert — soft-deleted; GetAll should not return it
        var result = await _companyAppService.GetAllAsync(
            new PagedCompanyResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.Items.ShouldNotContain(c => c.Id == created.Id);
    }

    [Fact]
    public async Task Tenant_Isolation_Company_TenantId_Stamped_Correctly()
    {
        // Arrange — create a company as tenant 1 (default, already logged in as TenantId=1)
        var created = await _companyAppService.CreateAsync(
            new CreateCompanyDto { Name = "Tenant1 Company", IsActive = true });

        // Assert — verify directly in DB that TenantId is stamped and data is scoped
        await UsingDbContextAsync(async context =>
        {
            var company = await context.Set<DMS.Companies.Company>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == created.Id);

            company.ShouldNotBeNull();
            company.TenantId.ShouldBe(AbpSession.TenantId.Value);

            // Verify a query scoped to a different TenantId returns nothing
            var otherTenantCompanies = await context.Set<DMS.Companies.Company>()
                .IgnoreQueryFilters()
                .Where(c => c.TenantId == 999 && c.Id == created.Id)
                .ToListAsync();

            otherTenantCompanies.ShouldBeEmpty();
        });
    }
}
