using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Payments.Dto;
using System.Threading.Tasks;

namespace DMS.Payments;

public interface IPaymentAppService
{
    Task<ApiResponse<PaymentDto>> RecordPaymentAsync(RecordPaymentDto input);
    Task<ApiResponse<PaymentDto>> GetAsync(int id);
    Task<ApiResponse<PagedResultDto<PaymentDto>>> GetAllAsync(PagedPaymentRequestDto input);
    Task<ApiResponse<byte[]>> GetReceiptBytesAsync(int paymentId);
    Task<ApiResponse<byte[]>> RegenerateReceiptAsync(int paymentId);
}
