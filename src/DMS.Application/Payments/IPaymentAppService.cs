using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DMS.Payments.Dto;
using System.Threading.Tasks;

namespace DMS.Payments;

public interface IPaymentAppService : IApplicationService
{
    Task<PaymentDto> RecordPaymentAsync(RecordPaymentDto input);
    Task<PaymentDto> GetAsync(int id);
    Task<PagedResultDto<PaymentDto>> GetAllAsync(PagedPaymentRequestDto input);
    Task<byte[]> GetReceiptBytesAsync(int paymentId);
    Task<byte[]> RegenerateReceiptAsync(int paymentId);
}
