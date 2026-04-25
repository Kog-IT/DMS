using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.ProductGroups.Dto;

[AutoMapTo(typeof(ProductGroup))]
public class CreateProductGroupDto
{
    [Required]
    [StringLength(ProductGroup.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(ProductGroup.MaxNameLength)]
    public string Name_EN { get; set; }

    public bool IsActive { get; set; } = true;
}
