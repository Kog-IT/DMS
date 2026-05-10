using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Brands.Dto;

[AutoMapTo(typeof(Brand))]
public class CreateBrandDto
{
    [Required]
    [StringLength(Brand.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Brand.MaxNameLength)]
    public string Name_EN { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> Paths { get; set; } = new List<string>();
}
