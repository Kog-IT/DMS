using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Transfers;

public class WarehouseToWarehouseTransfer : FullAuditedEntity<int>, IMustHaveTenant
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime TransferDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; }

    public int Status { get; set; } = 0;
    public int TenantId { get; set; }
}
