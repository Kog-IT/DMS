using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments;

public class PaymentLine : Entity<int>, IMustHaveTenant
{
    public const int MaxReferenceLength = 256;

    public int TenantId { get; set; }
    public int PaymentId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }

    [StringLength(MaxReferenceLength)]
    public string Reference { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; }
}
