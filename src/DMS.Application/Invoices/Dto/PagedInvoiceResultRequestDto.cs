using Abp.Application.Services.Dto;
using System;

namespace DMS.Invoices.Dto;

public class PagedInvoiceResultRequestDto : PagedResultRequestDto
{
    public int? CustomerId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
