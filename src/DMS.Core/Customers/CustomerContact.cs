using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers;

public class CustomerContact : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 128;
    public const int MaxPhoneLength = 32;
    public const int MaxEmailLength = 256;
    public const int MaxTitleLength = 128;
    public const int MaxWhatsAppLength = 32;
    public const int MaxSocialHandleLength = 128;
    public const int MaxContactsPerCustomer = 10;

    public int TenantId { get; set; }

    public int CustomerId { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(MaxWhatsAppLength)]
    public string WhatsApp { get; set; }

    [StringLength(MaxSocialHandleLength)]
    public string SocialHandle { get; set; }

    public bool IsPrimary { get; set; } = false;
}
