using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.CustomerGroups;

public class CustomerGroup : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 100;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    public bool IsTaxExempted { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
