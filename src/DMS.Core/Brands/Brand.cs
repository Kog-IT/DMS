using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Brands;

public class Brand : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 128;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxNameLength)]
    public string Name_EN { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
