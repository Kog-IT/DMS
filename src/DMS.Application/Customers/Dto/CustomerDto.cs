using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;
using System;

namespace DMS.Customers.Dto;

[AutoMapFrom(typeof(Customer))]
public class CustomerDto : FullAuditedEntityDto<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public CustomerClassification Classification { get; set; }
    public DateTime? LastClassifiedAt { get; set; }
    public decimal CreditLimit { get; set; }
    public bool CreditEnabled { get; set; }
}
