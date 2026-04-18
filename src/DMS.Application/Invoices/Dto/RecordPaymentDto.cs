using System.ComponentModel.DataAnnotations;

namespace DMS.Invoices.Dto;

public class RecordPaymentDto
{
    [Required]
    public int InvoiceId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}
