using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.SalesmanRequests;

public class SalesmanRequest : FullAuditedEntity<int>, IMustHaveTenant
{
    public int SalesmanId { get; set; }

    public int WarehouseId { get; set; }

    public DateTime RequestDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

    public int Status { get; set; } = 0;

    [StringLength(500)]
    public string RejectionReason { get; set; }

    public int TenantId { get; set; }
}
