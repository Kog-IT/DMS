using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Invoices.Dto;

public class InvoiceDto : EntityDto<int>
{
    public int OrderId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public string Notes { get; set; }
    public string VoidReason { get; set; }
    public List<InvoiceLineDto> Lines { get; set; } = new();
}
