using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Companies;

namespace DMS.Companies.Dto;

[AutoMapFrom(typeof(Company))]
public class CompanyDto : FullAuditedEntityDto<int>
{
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}
