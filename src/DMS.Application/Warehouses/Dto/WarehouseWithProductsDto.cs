using System.Collections.Generic;

namespace DMS.Warehouses.Dto;

public class WarehouseWithProductsDto : WarehouseDto
{
    public List<WarehouseProductDto> WarehouseProducts { get; set; } = new();
}
