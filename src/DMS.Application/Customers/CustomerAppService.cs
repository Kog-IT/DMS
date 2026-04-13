using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Customers.Dto;
using System.Linq;

namespace DMS.Customers;

[AbpAuthorize(PermissionNames.Pages_Customers)]
public class CustomerAppService : AsyncCrudAppService<
    Customer,
    CustomerDto,
    int,
    PagedCustomerResultRequestDto,
    CreateCustomerDto,
    UpdateCustomerDto>, ICustomerAppService
{
    public CustomerAppService(IRepository<Customer, int> repository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Customers;
        GetAllPermissionName = PermissionNames.Pages_Customers;
        CreatePermissionName = PermissionNames.Pages_Customers_Create;
        UpdatePermissionName = PermissionNames.Pages_Customers_Edit;
        DeletePermissionName = PermissionNames.Pages_Customers_Delete;
    }

    protected override IQueryable<Customer> CreateFilteredQuery(PagedCustomerResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(
                !input.Keyword.IsNullOrWhiteSpace(),
                c => c.Name.Contains(input.Keyword) || c.Code.Contains(input.Keyword)
            );
    }
}
