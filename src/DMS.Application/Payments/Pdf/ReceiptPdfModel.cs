using System.Collections.Generic;

namespace DMS.Payments.Pdf;

public class ReceiptPdfModel
{
    public string ReceiptNumber { get; set; }
    public string PaymentDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal InvoiceTotal { get; set; }
    public decimal PaidBeforeThis { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingBalance { get; set; }
    public string InvoiceStatus { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string? CompanyLogoPath { get; set; }
    public List<ReceiptLineModel> Lines { get; set; } = new();
}

public class ReceiptLineModel
{
    public string MethodName { get; set; }
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
}
