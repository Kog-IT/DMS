using Abp.AutoMapper;
using DMS.Warehouses;
using System.ComponentModel.DataAnnotations;

namespace DMS.Warehouses.Dto;

[AutoMapTo(typeof(Warehouse))]
public class CreateWarehouseDto
{
    [Required]
    [StringLength(Warehouse.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(Warehouse.MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(Warehouse.MaxDataLength)]
    public string? Data { get; set; }

    public int WarehouseType { get; set; } = 0;

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    [StringLength(Warehouse.MaxStreetLength)]
    public string? Street { get; set; }

    [StringLength(Warehouse.MaxLandmarkLength)]
    public string? Landmark { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    [StringLength(Warehouse.MaxBuildingDataLength)]
    public string? BuildingData { get; set; }

    public bool IsActive { get; set; } = true;
}
