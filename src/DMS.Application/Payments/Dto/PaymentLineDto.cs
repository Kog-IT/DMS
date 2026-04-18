using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace DMS.Payments.Dto;

public class PaymentLineDto : EntityDto<int>
{
    public int PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string? Reference { get; set; }
}

public class CreatePaymentLineDto
{
    [Required]
    public int PaymentMethodId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string? Reference { get; set; }
}
