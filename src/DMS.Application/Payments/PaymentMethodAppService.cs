using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Payments.Dto;
using System.Linq;

namespace DMS.Payments;

[AbpAuthorize(PermissionNames.Pages_PaymentMethods)]
public class PaymentMethodAppService : AsyncCrudAppService<
    PaymentMethod,
    PaymentMethodDto,
    int,
    PagedPaymentMethodResultRequestDto,
    CreatePaymentMethodDto,
    UpdatePaymentMethodDto>, IPaymentMethodAppService
{
    public PaymentMethodAppService(IRepository<PaymentMethod, int> repository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_PaymentMethods;
        GetAllPermissionName = PermissionNames.Pages_PaymentMethods;
        CreatePermissionName = PermissionNames.Pages_PaymentMethods_Create;
        UpdatePermissionName = PermissionNames.Pages_PaymentMethods_Edit;
        DeletePermissionName = PermissionNames.Pages_PaymentMethods_Delete;
    }

    protected override IQueryable<PaymentMethod> CreateFilteredQuery(PagedPaymentMethodResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                m => m.Name.Contains(input.Keyword) || m.Code.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue,
                m => m.IsActive == input.IsActive.Value);
    }

    protected override IQueryable<PaymentMethod> ApplySorting(IQueryable<PaymentMethod> query, PagedPaymentMethodResultRequestDto input)
    {
        return query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Name);
    }
}
