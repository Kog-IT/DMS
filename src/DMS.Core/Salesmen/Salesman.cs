using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Salesmen;

public class Salesman : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 200;
    public const int MaxJobCodeLength = 50;
    public const int MaxMobileLength = 20;
    public const int MaxEmailLength = 150;
    public const int MaxNationalNumberLength = 30;
    public const int MaxAddressLength = 250;
    public const int MaxPathLength = 500;
    public const int MaxSalesSupervisorIdLength = 50;
    public const int MaxUserNameLength = 150;

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxJobCodeLength)]
    public string JobCode { get; set; }

    [StringLength(MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(MaxNationalNumberLength)]
    public string NationalNumber { get; set; }

    [StringLength(MaxAddressLength)]
    public string Address { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? AppointmentDate { get; set; }

    [StringLength(MaxPathLength)]
    public string Path { get; set; }

    [StringLength(MaxSalesSupervisorIdLength)]
    public string SalesSupervisorId { get; set; }

    [StringLength(MaxUserNameLength)]
    public string UserName { get; set; }

    public long? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public int TenantId { get; set; }
}
