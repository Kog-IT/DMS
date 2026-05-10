using Abp.Application.Services.Dto;
using DMS.MultiTenancy;
using DMS.MultiTenancy.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.MultiTenancy;

public class TenantAppService_Tests : DMSTestBase
{
    private readonly ITenantAppService _tenantAppService;

    public TenantAppService_Tests()
    {
        LoginAsHostAdmin();
        _tenantAppService = Resolve<ITenantAppService>();
    }

    [Fact]
    public async Task Tenant_ImageUrl_ShouldPersistToDatabase()
    {
        int tenantId = 0;
        await UsingDbContextAsync(null, async context =>
        {
            var tenant = new Tenant("imgtest", "Image Test Tenant")
            {
                ImageUrl = "https://example.com/logo.png",
                IsActive = true
            };
            await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync();
            tenantId = tenant.Id;
        });

        await UsingDbContextAsync(null, async context =>
        {
            var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            tenant.ShouldNotBeNull();
            tenant.ImageUrl.ShouldBe("https://example.com/logo.png");
        });
    }

    [Fact]
    public async Task UpdateTenantImage_ShouldUpdate_ImageUrl()
    {
        // Arrange: create a known tenant
        var created = await _tenantAppService.CreateAsync(new CreateTenantDto
        {
            TenancyName = "imgupdate",
            Name = "Image Update Test",
            AdminEmailAddress = "admin@imgupdate.test",
            IsActive = true
        });
        created.ShouldNotBeNull();
        created.Data.ShouldNotBeNull();

        // Act
        var result = await _tenantAppService.UpdateTenantImageAsync(new UpdateTenantImageDto
        {
            Id = created.Data.Id,
            ImageUrl = "https://example.com/new-logo.png"
        });

        // Assert response
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ImageUrl.ShouldBe("https://example.com/new-logo.png");

        // Assert DB persistence
        await UsingDbContextAsync(null, async context =>
        {
            var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == created.Data.Id);
            tenant.ShouldNotBeNull();
            tenant.ImageUrl.ShouldBe("https://example.com/new-logo.png");
        });
    }
}
