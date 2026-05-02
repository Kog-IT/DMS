using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Suppliers;

public class Supplier : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 200;
    public const int MaxCodeLength = 50;
    public const int MaxMobileLength = 20;
    public const int MaxEmailLength = 150;
    public const int MaxAddressLength = 300;
    public const int MaxCollaborationAdministratorLength = 200;
    public const int MaxPathLength = 500;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(MaxAddressLength)]
    public string Address { get; set; }

    [StringLength(MaxCollaborationAdministratorLength)]
    public string CollaborationAdministrator { get; set; }

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    [StringLength(MaxPathLength)]
    public string Path { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
