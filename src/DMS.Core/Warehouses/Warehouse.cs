// src/DMS.Core/Warehouses/Warehouse.cs
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Warehouses;

public class Warehouse : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 128;
    public const int MaxCodeLength = 50;
    public const int MaxDataLength = 500;
    public const int MaxStreetLength = 200;
    public const int MaxLandmarkLength = 200;
    public const int MaxCoordLength = 50;
    public const int MaxBuildingDataLength = 200;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(MaxDataLength)]
    public string? Data { get; set; }

    public int WarehouseType { get; set; } = 0;

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    [StringLength(MaxStreetLength)]
    public string? Street { get; set; }

    [StringLength(MaxLandmarkLength)]
    public string? Landmark { get; set; }

    [StringLength(MaxCoordLength)]
    public string? Latitude { get; set; }

    [StringLength(MaxCoordLength)]
    public string? Longitude { get; set; }

    [StringLength(MaxBuildingDataLength)]
    public string? BuildingData { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
