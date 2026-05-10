// src/DMS.Core/Warehouses/WarehouseProduct.cs
using Abp.Domain.Entities;

namespace DMS.Warehouses;

public class WarehouseProduct : Entity<int>, IMustHaveTenant
{
    public int WarehouseId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal WeightPerKG { get; set; }
    public int TenantId { get; set; }
}
