using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Warehouses;

namespace DMS.Warehouses.Dto;

[AutoMapFrom(typeof(Warehouse))]
public class WarehouseDto : EntityDto<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Data { get; set; }
    public int WarehouseType { get; set; }
    public int GovernorateId { get; set; }
    public int CityId { get; set; }
    public string? Street { get; set; }
    public string? Landmark { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? BuildingData { get; set; }
    public bool IsActive { get; set; }
}
