using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Categories;

namespace DMS.Categories.Dto
{
    [AutoMapFrom(typeof(Category))]
    public class CategoryDto : FullAuditedEntityDto<int>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
