using Abp.AutoMapper;
using DMS.Customers;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers.Dto;

[AutoMapTo(typeof(Customer))]
public class CreateCustomerDto
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

    public bool IsActive { get; set; } = true;

    [Range(0, double.MaxValue)]
    public decimal CreditLimit { get; set; } = 0m;

    public bool CreditEnabled { get; set; } = false;
}
