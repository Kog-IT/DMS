using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Transfers;

public class SalesmanToWarehouseTransfer : FullAuditedEntity<int>, IMustHaveTenant
{
    public int SalesmanId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime TransferDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

    public int Status { get; set; } = 0;

    [StringLength(500)]
    public string RejectionReason { get; set; }

    public int TenantId { get; set; }
}
