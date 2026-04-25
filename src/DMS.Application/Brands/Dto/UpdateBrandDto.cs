using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Brands.Dto;

[AutoMapTo(typeof(Brand))]
public class UpdateBrandDto : EntityDto<int>
{
    [Required]
    [StringLength(Brand.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Brand.MaxNameLength)]
    public string Name_EN { get; set; }

    public bool IsActive { get; set; }
}
