using Abp.Application.Services;
using DMS.Payments.Dto;

namespace DMS.Payments;

public interface IPaymentMethodAppService : IAsyncCrudAppService<
    PaymentMethodDto,
    int,
    PagedPaymentMethodResultRequestDto,
    CreatePaymentMethodDto,
    UpdatePaymentMethodDto>
{
}
