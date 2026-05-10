using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.CustomerGroups.Dto;

[AutoMapTo(typeof(CustomerGroup))]
public class CreateCustomerGroupDto
{
    [Required]
    [StringLength(CustomerGroup.MaxNameLength)]
    public string Name { get; set; }

    public bool IsTaxExempted { get; set; } = false;

    public bool IsActive { get; set; } = true;
}
