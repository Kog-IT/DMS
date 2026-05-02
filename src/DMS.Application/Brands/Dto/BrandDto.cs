using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;

namespace DMS.Brands.Dto;

[AutoMapFrom(typeof(Brand))]
public class BrandDto : EntityDto<int>
{
    public string Name { get; set; }
    public string Name_EN { get; set; }
    public bool IsActive { get; set; }
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
