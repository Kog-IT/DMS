using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments;

public class Payment : CreationAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxReceiptNumberLength = 64;
    public const int MaxNotesLength = 512;
    public const int MaxReceiptFilePathLength = 512;

    public int TenantId { get; set; }
    public int InvoiceId { get; set; }

    [Required]
    [StringLength(MaxReceiptNumberLength)]
    public string ReceiptNumber { get; set; }

    public DateTime PaymentDate { get; set; }
    public decimal TotalAmount { get; set; }

    [StringLength(MaxNotesLength)]
    public string Notes { get; set; }

    [StringLength(MaxReceiptFilePathLength)]
    public string ReceiptFilePath { get; set; }

    public virtual ICollection<PaymentLine> Lines { get; set; } = new List<PaymentLine>();
}
