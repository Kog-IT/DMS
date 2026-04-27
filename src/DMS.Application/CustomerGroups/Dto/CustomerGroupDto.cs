using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.CustomerGroups.Dto;

[AutoMapFrom(typeof(CustomerGroup))]
public class CustomerGroupDto : EntityDto<int>
{
    public string Name { get; set; }
    public bool IsTaxExempted { get; set; }
    public bool IsActive { get; set; }
}
