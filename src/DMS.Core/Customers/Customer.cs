using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers;

public class Customer : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxCodeLength = 64;
    public const int MaxNameLength = 256;
    public const int MaxAddressLength = 512;
    public const int MaxPhoneLength = 32;
    public const int MaxEmailLength = 256;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxAddressLength)]
    public string Address { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [StringLength(MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    public bool IsActive { get; set; } = true;

    public CustomerClassification Classification { get; set; } = CustomerClassification.Unclassified;
    public DateTime? LastClassifiedAt { get; set; }

    public decimal CreditLimit { get; set; } = 0m;
    public bool CreditEnabled { get; set; } = false;
    public int CreditDays { get; set; } = 0;
    public bool IsBlocked { get; set; } = false;
}
