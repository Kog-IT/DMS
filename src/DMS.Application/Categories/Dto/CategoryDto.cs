using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Categories;
using System.Collections.Generic;

namespace DMS.Categories.Dto
{
    [AutoMapFrom(typeof(Category))]
    public class CategoryDto : FullAuditedEntityDto<int>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
    }
}
