using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments.Dto;

public class RecordPaymentDto
{
    [Required]
    public int InvoiceId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreatePaymentLineDto> Lines { get; set; } = new();
}
