using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;
using System;
using System.Collections.Generic;

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
    public int CreditDays { get; set; }
    public bool IsBlocked { get; set; }
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
