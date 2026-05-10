using Abp.AutoMapper;
using DMS.Warehouses;

namespace DMS.Warehouses.Dto;

[AutoMapFrom(typeof(WarehouseProduct))]
public class WarehouseProductDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal WeightPerKG { get; set; }
}
