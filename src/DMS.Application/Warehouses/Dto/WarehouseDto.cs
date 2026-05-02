using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Warehouses;
using System.Collections.Generic;

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
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? BuildingData { get; set; }
    public bool IsActive { get; set; }
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
