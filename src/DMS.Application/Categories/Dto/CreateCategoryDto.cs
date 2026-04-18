using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Categories.Dto;

[AutoMapTo(typeof(Category))]
public class CreateCategoryDto
{
    [Required]
    [StringLength(Category.MaxNameLength)]
    public string Name { get; set; }
}
