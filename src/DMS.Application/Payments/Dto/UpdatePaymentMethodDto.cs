using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments.Dto;

[AutoMapTo(typeof(PaymentMethod))]
public class UpdatePaymentMethodDto : EntityDto<int>
{
    [Required]
    [StringLength(PaymentMethod.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(PaymentMethod.MaxCodeLength)]
    public string Code { get; set; }

    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
