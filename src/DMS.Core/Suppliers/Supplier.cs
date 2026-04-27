using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Suppliers;

public class Supplier : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 200;
    public const int MaxCodeLength = 50;
    public const int MaxPhoneLength = 20;
    public const int MaxEmailLength = 150;
    public const int MaxAddressLength = 300;
    public const int MaxImageUrlLength = 500;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(MaxAddressLength)]
    public string Address { get; set; }

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    [StringLength(MaxImageUrlLength)]
    public string ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
