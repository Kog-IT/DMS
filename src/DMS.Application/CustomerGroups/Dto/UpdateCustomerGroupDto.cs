using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.CustomerGroups.Dto;

[AutoMapTo(typeof(CustomerGroup))]
public class UpdateCustomerGroupDto : EntityDto<int>
{
    [Required]
    [StringLength(CustomerGroup.MaxNameLength)]
    public string Name { get; set; }

    public bool IsTaxExempted { get; set; }

    public bool IsActive { get; set; }
}
