using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Categories.Dto;

[AutoMapTo(typeof(Category))]
public class UpdateCategoryDto : EntityDto<int>
{
    [Required]
    [StringLength(Category.MaxNameLength)]
    public string Name { get; set; }
}
