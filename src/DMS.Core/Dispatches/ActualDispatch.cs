using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Dispatches;

public class ActualDispatch : FullAuditedEntity<int>, IMustHaveTenant
{
    public int SalesmanId { get; set; }

    public DateTime DispatchDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

    public decimal TotalAmount { get; set; } = 0;

    public int TenantId { get; set; }
}
