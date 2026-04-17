using Abp.Application.Services.Dto;
using System;

namespace DMS.Payments.Dto;

public class PagedPaymentRequestDto : PagedResultRequestDto
{
    public int? InvoiceId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
