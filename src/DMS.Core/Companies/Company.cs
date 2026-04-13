using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace DMS.Companies;

public class Company : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 256;
    public const int MaxTaxNumberLength = 64;
    public const int MaxAddressLength = 512;
    public const int MaxPhoneLength = 32;
    public const int MaxEmailLength = 256;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxTaxNumberLength)]
    public string TaxNumber { get; set; }

    [StringLength(MaxAddressLength)]
    public string Address { get; set; }

    [StringLength(MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    public bool IsActive { get; set; } = true;
}
