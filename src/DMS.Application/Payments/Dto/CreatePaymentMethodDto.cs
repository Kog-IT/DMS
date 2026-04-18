using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments.Dto;

[AutoMapTo(typeof(PaymentMethod))]
public class CreatePaymentMethodDto
{
    [Required]
    [StringLength(PaymentMethod.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(PaymentMethod.MaxCodeLength)]
    public string Code { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
