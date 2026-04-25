using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Customers.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Customers;

[AbpAuthorize(PermissionNames.Pages_Customers)]
public class CustomerAppService : DmsCrudAppService<
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
    public async Task<ApiResponse<CreditStatusDto>> GetCreditStatusAsync(int customerId)
    {
        var customer = await Repository.GetAsync(customerId);
        var result = await _creditCheckService.CheckCreditAsync(customerId, orderTotal: 0m);

        var dto = new CreditStatusDto
        {
            CustomerId = customerId,
            CreditEnabled = customer.CreditEnabled,
            IsBlocked = customer.IsBlocked,
            CreditDays = customer.CreditDays,
            CreditLimit = result.CreditLimit,
            OutstandingBalance = result.OutstandingBalance,
            AvailableCredit = result.AvailableCredit,
            UtilizationPercent = result.UtilizationPercent
        };

        return Ok(dto, L("RetrievedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Customers_Block)]
    public async Task<ApiResponse<object>> BlockAsync(int customerId, string reason)
    {
        var customer = await Repository.GetAsync(customerId);
        if (customer.IsBlocked)
            throw new UserFriendlyException("Customer is already blocked.");

        customer.IsBlocked = true;
        await Repository.UpdateAsync(customer);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Customers_Block)]
    public async Task<ApiResponse<object>> UnblockAsync(int customerId)
    {
        var customer = await Repository.GetAsync(customerId);
        if (!customer.IsBlocked)
            throw new UserFriendlyException("Customer is not blocked.");

        customer.IsBlocked = false;
        await Repository.UpdateAsync(customer);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Customers_ManageCredit)]
    public async Task<ApiResponse<object>> UpdateCreditLimitAsync(int customerId, UpdateCreditLimitDto input)
    {
        var customer = await Repository.GetAsync(customerId);
        customer.CreditLimit = input.CreditLimit;
        customer.CreditEnabled = input.CreditEnabled;
        customer.CreditDays = input.CreditDays;
        await Repository.UpdateAsync(customer);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }
}
