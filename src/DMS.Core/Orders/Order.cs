using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Orders;

public class Order : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNotesLength = 1024;
    public const int MaxRejectionReasonLength = 512;

    public int TenantId { get; set; }
    public int CustomerId { get; set; }
    public int? VisitId { get; set; }
    public long AssignedUserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public DateTime OrderDate { get; set; }

    [StringLength(MaxNotesLength)]
    public string Notes { get; set; }

    public DiscountType OrderDiscountType { get; set; } = DiscountType.None;
    public decimal OrderDiscountValue { get; set; } = 0;
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal OrderDiscountAmount { get; set; }
    public decimal Total { get; set; }

    [StringLength(MaxRejectionReasonLength)]
    public string RejectionReason { get; set; }

    public virtual ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
}
