using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Payments.Dto;

public class PaymentDto : EntityDto<int>
{
    public int InvoiceId { get; set; }
    public string ReceiptNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptFilePath { get; set; }
    public List<PaymentLineDto> Lines { get; set; } = new();
}
