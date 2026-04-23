using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Customers.Dto;
using Microsoft.EntityFrameworkCore;

namespace DMS.Customers;

[AbpAuthorize(PermissionNames.Pages_Customers_Contacts)]
public class CustomerContactAppService : DmsCrudAppService<
    CustomerContact,
    CustomerContactDto,
    int,
    PagedCustomerContactResultRequestDto,
    CreateCustomerContactDto,
    UpdateCustomerContactDto>, ICustomerContactAppService
{
    private readonly IRepository<Customer, int> _customerRepository;

    public CustomerContactAppService(
        IRepository<CustomerContact, int> repository,
        IRepository<Customer, int> customerRepository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Customers_Contacts;
        GetAllPermissionName = PermissionNames.Pages_Customers_Contacts;
        CreatePermissionName = PermissionNames.Pages_Customers_Contacts_Create;
        UpdatePermissionName = PermissionNames.Pages_Customers_Contacts_Edit;
        DeletePermissionName = PermissionNames.Pages_Customers_Contacts_Delete;
        _customerRepository = customerRepository;
    }

    protected override IQueryable<CustomerContact> CreateFilteredQuery(PagedCustomerContactResultRequestDto input)
    {
        return Repository.GetAll()
            .Where(c => c.CustomerId == input.CustomerId);
    }

    public override async Task<ApiResponse<CustomerContactDto>> CreateAsync(CreateCustomerContactDto input)
    {
        var customerExists = await _customerRepository.GetAll()
            .AnyAsync(c => c.Id == input.CustomerId);
        if (!customerExists)
            throw new UserFriendlyException("Customer not found.");

        var count = await Repository.CountAsync(c => c.CustomerId == input.CustomerId);
        if (count >= CustomerContact.MaxContactsPerCustomer)
            throw new UserFriendlyException("Customer cannot have more than 10 contacts.");

        if (input.IsPrimary)
            await UnsetPrimaryAsync(input.CustomerId);

        return await base.CreateAsync(input);
    }

    public override async Task<ApiResponse<CustomerContactDto>> UpdateAsync(UpdateCustomerContactDto input)
    {
        var existing = await Repository.GetAsync(input.Id);

        if (input.IsPrimary && !existing.IsPrimary)
            await UnsetPrimaryAsync(existing.CustomerId, existing.Id);

        return await base.UpdateAsync(input);
    }

    private async Task UnsetPrimaryAsync(int customerId, int excludeContactId = 0)
    {
        var primaries = await Repository.GetAll()
            .Where(c => c.CustomerId == customerId && c.IsPrimary && c.Id != excludeContactId)
            .ToListAsync();

        foreach (var contact in primaries)
        {
            contact.IsPrimary = false;
        }

        await CurrentUnitOfWork.SaveChangesAsync();
    }
}
