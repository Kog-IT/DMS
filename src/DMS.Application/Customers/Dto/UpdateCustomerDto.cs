using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers.Dto;

[AutoMapTo(typeof(Customer))]
public class UpdateCustomerDto : EntityDto<int>
{
    [Required]
    [StringLength(Customer.MaxCodeLength)]
    public string Code { get; set; }

    [Required]
    [StringLength(Customer.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Customer.MaxAddressLength)]
    public string Address { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [StringLength(Customer.MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(Customer.MaxEmailLength)]
    public string Email { get; set; }

    /// <summary>Full-replace semantics: client must always send the complete object. IsActive=false deactivates the customer.</summary>
    public bool IsActive { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CreditLimit { get; set; }

    public bool CreditEnabled { get; set; }
}
