using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.UI;
using DMS.Customers;
using DMS.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Customers;

public class CustomerContact_Tests : DMSTestBase
{
    private readonly ICustomerContactAppService _contactService;

    public CustomerContact_Tests()
    {
        _contactService = Resolve<ICustomerContactAppService>();
    }

    private async Task<int> SeedCustomerAsync(string code)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var c = new Customer
            {
                TenantId = 1, Code = code, Name = "Test Customer", IsActive = true
            };
            ctx.Set<Customer>().Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        });
        return id;
    }

    [Fact]
    public async Task Create_Contact_Succeeds()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C1");

        var result = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId,
            Name = "John Doe",
            Phone = "555-1234",
            Email = "john@example.com",
            Title = "Manager"
        });

        result.Id.ShouldBeGreaterThan(0);
        result.Name.ShouldBe("John Doe");
        result.CustomerId.ShouldBe(customerId);
    }

    [Fact]
    public async Task Create_Eleventh_Contact_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C2");

        for (int i = 1; i <= 10; i++)
        {
            await _contactService.CreateAsync(new CreateCustomerContactDto
            {
                CustomerId = customerId, Name = $"Contact {i}"
            });
        }

        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _contactService.CreateAsync(new CreateCustomerContactDto
            {
                CustomerId = customerId, Name = "Contact 11"
            })
        );
    }

    [Fact]
    public async Task Create_With_IsPrimary_Unsets_Previous_Primary()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C3");

        var first = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "First", IsPrimary = true
        });

        var second = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "Second", IsPrimary = true
        });

        await UsingDbContextAsync(async ctx =>
        {
            var firstContact = await ctx.Set<CustomerContact>().FindAsync(first.Id);
            var secondContact = await ctx.Set<CustomerContact>().FindAsync(second.Id);
            firstContact.IsPrimary.ShouldBeFalse();
            secondContact.IsPrimary.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Update_Contact_Succeeds()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C4");

        var created = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "Original"
        });

        var updated = await _contactService.UpdateAsync(new UpdateCustomerContactDto
        {
            Id = created.Id,
            Name = "Updated Name",
            Phone = "999-0000"
        });

        updated.Name.ShouldBe("Updated Name");
        updated.Phone.ShouldBe("999-0000");
    }

    [Fact]
    public async Task Delete_Contact_Succeeds()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C5");

        var created = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "To Delete"
        });

        await _contactService.DeleteAsync(new EntityDto<int>(created.Id));

        await UsingDbContextAsync(async ctx =>
        {
            var contact = await ctx.Set<CustomerContact>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == created.Id);
            contact.IsDeleted.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task GetAll_Returns_Only_Contacts_For_Customer()
    {
        LoginAsDefaultTenantAdmin();
        var customer1 = await SeedCustomerAsync("CC_C6A");
        var customer2 = await SeedCustomerAsync("CC_C6B");

        await _contactService.CreateAsync(new CreateCustomerContactDto { CustomerId = customer1, Name = "C1 Contact" });
        await _contactService.CreateAsync(new CreateCustomerContactDto { CustomerId = customer2, Name = "C2 Contact" });

        var result = await _contactService.GetAllAsync(new PagedCustomerContactResultRequestDto
        {
            CustomerId = customer1,
            MaxResultCount = 10,
            SkipCount = 0
        });

        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("C1 Contact");
    }

    [Fact]
    public async Task Create_With_Unknown_CustomerId_Throws()
    {
        LoginAsDefaultTenantAdmin();

        await Should.ThrowAsync<Abp.UI.UserFriendlyException>(async () =>
            await _contactService.CreateAsync(new CreateCustomerContactDto
            {
                CustomerId = 99999,
                Name = "Ghost Contact"
            })
        );
    }

    [Fact]
    public async Task Update_With_IsPrimary_Unsets_Previous_Primary()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("CC_C7");

        var first = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "First", IsPrimary = true
        });

        var second = await _contactService.CreateAsync(new CreateCustomerContactDto
        {
            CustomerId = customerId, Name = "Second", IsPrimary = false
        });

        // Promote second to primary via update
        await _contactService.UpdateAsync(new UpdateCustomerContactDto
        {
            Id = second.Id, Name = "Second", IsPrimary = true
        });

        await UsingDbContextAsync(async ctx =>
        {
            var firstContact = await ctx.Set<CustomerContact>().FindAsync(first.Id);
            var secondContact = await ctx.Set<CustomerContact>().FindAsync(second.Id);
            firstContact.IsPrimary.ShouldBeFalse();
            secondContact.IsPrimary.ShouldBeTrue();
        });
    }
}
