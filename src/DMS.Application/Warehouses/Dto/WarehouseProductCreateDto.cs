using System.Collections.Generic;

namespace DMS.Warehouses.Dto;

public class WarehouseProductCreateDto
{
    public int WarehouseId { get; set; }
    public List<ProductItemDto> ProductWarehouses { get; set; } = new();
}
