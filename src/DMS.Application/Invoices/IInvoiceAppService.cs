using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Invoices.Dto;
using System.Threading.Tasks;

namespace DMS.Invoices;

public interface IInvoiceAppService
{
    Task<ApiResponse<InvoiceDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<InvoiceDto>>> GetAllAsync(PagedInvoiceResultRequestDto input);
    Task<ApiResponse<InvoiceDto>> CreateAsync(GenerateInvoiceDto input);
    Task<ApiResponse<InvoiceDto>> UpdateAsync(GenerateInvoiceDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<InvoiceDto>> GenerateFromOrderAsync(int orderId);
    Task<ApiResponse<object>> IssueAsync(int id);
    Task<ApiResponse<object>> VoidAsync(VoidInvoiceDto input);
}
