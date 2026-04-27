using Abp.Domain.Entities;

namespace DMS.Transfers;

public class SalesmanToWarehouseTransferItem : Entity<int>, IMustHaveTenant
{
    public int TransferId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public int TenantId { get; set; }
}
