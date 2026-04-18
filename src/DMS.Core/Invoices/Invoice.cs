using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DMS.Orders;

namespace DMS.Invoices;

public class Invoice : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxInvoiceNumberLength = 64;
    public const int MaxCustomerNameLength = 256;
    public const int MaxCustomerAddressLength = 512;
    public const int MaxNotesLength = 1024;
    public const int MaxVoidReasonLength = 512;

    public int TenantId { get; set; }
    public int OrderId { get; set; }

    [Required]
    [StringLength(MaxInvoiceNumberLength)]
    public string InvoiceNumber { get; set; }

    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    [StringLength(MaxCustomerNameLength)]
    public string CustomerName { get; set; }

    [StringLength(MaxCustomerAddressLength)]
    public string CustomerAddress { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; } = 0;

    [StringLength(MaxNotesLength)]
    public string Notes { get; set; }

    [StringLength(MaxVoidReasonLength)]
    public string VoidReason { get; set; }

    public virtual ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}
