using System.ComponentModel.DataAnnotations;

namespace DMS.Invoices.Dto;

public class VoidInvoiceDto
{
    [Required]
    public int InvoiceId { get; set; }

    [Required]
    [StringLength(512)]
    public string Reason { get; set; }
}
