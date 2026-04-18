using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Customers.Dto;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly CreditCheckService _creditCheckService;

    public CustomerAppService(
        IRepository<Customer, int> repository,
        CreditCheckService creditCheckService)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Customers;
        GetAllPermissionName = PermissionNames.Pages_Customers;
        CreatePermissionName = PermissionNames.Pages_Customers_Create;
        UpdatePermissionName = PermissionNames.Pages_Customers_Edit;
        DeletePermissionName = PermissionNames.Pages_Customers_Delete;

        _creditCheckService = creditCheckService;
    }

    protected override IQueryable<Customer> CreateFilteredQuery(PagedCustomerResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(
                !input.Keyword.IsNullOrWhiteSpace(),
                c => c.Name.Contains(input.Keyword) || c.Code.Contains(input.Keyword)
            );
    }

    [AbpAuthorize(PermissionNames.Pages_Customers)]
    public async Task<CreditStatusDto> GetCreditStatusAsync(int customerId)
    {
        var customer = await Repository.GetAsync(customerId);
        var result = await _creditCheckService.CheckCreditAsync(customerId, orderTotal: 0m);

        return new CreditStatusDto
        {
            CustomerId = customerId,
            CreditEnabled = customer.CreditEnabled,
            CreditLimit = result.CreditLimit,
            OutstandingBalance = result.OutstandingBalance,
            AvailableCredit = result.AvailableCredit
        };
    }
}
