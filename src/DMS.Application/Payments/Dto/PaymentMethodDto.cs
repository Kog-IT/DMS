using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.Payments.Dto;

[AutoMapFrom(typeof(PaymentMethod))]
public class PaymentMethodDto : EntityDto<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
