using Abp.Domain.Entities;

namespace DMS.Salesmen;

public class SalesmanWarehouse : Entity<int>
{
    public int SalesmanId { get; set; }
    public int WarehouseId { get; set; }
}
