using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Companies;
using System.ComponentModel.DataAnnotations;

namespace DMS.Companies.Dto;

[AutoMapTo(typeof(Company))]
public class UpdateCompanyDto : EntityDto<int>
{
    [Required]
    [StringLength(Company.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Company.MaxTaxNumberLength)]
    public string TaxNumber { get; set; }

    [StringLength(Company.MaxAddressLength)]
    public string Address { get; set; }

    [StringLength(Company.MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(Company.MaxEmailLength)]
    public string Email { get; set; }

    public bool IsActive { get; set; }
}
