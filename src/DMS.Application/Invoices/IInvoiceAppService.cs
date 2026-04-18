using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DMS.Invoices.Dto;
using System.Threading.Tasks;

namespace DMS.Invoices;

public interface IInvoiceAppService : IAsyncCrudAppService<InvoiceDto, int, PagedInvoiceResultRequestDto, GenerateInvoiceDto, GenerateInvoiceDto>
{
    Task<InvoiceDto> GenerateFromOrderAsync(int orderId);
    Task IssueAsync(int id);
    Task VoidAsync(VoidInvoiceDto input);
}
