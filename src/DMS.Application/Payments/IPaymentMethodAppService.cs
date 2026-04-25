using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Payments.Dto;
using System.Threading.Tasks;

namespace DMS.Payments;

public interface IPaymentMethodAppService
{
    Task<ApiResponse<PaymentMethodDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<PaymentMethodDto>>> GetAllAsync(PagedPaymentMethodResultRequestDto input);
    Task<ApiResponse<PaymentMethodDto>> CreateAsync(CreatePaymentMethodDto input);
    Task<ApiResponse<PaymentMethodDto>> UpdateAsync(UpdatePaymentMethodDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
