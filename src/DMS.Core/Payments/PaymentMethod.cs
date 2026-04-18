using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments;

public class PaymentMethod : Entity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 128;
    public const int MaxCodeLength = 32;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(MaxCodeLength)]
    public string Code { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
