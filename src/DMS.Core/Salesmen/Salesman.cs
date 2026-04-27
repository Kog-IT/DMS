using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Salesmen;

public class Salesman : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 200;
    public const int MaxCodeLength = 50;
    public const int MaxMobileLength = 20;
    public const int MaxNationalIdLength = 30;
    public const int MaxImageUrlLength = 500;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(MaxNationalIdLength)]
    public string NationalId { get; set; }

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    public int? AssignedWarehouseId { get; set; }

    [StringLength(MaxImageUrlLength)]
    public string ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
