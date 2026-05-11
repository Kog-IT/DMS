using Abp.Application.Services.Dto;
using DMS.Cities.Dto;
using System.Collections.Generic;

namespace DMS.Governorates.Dto
{
    public class GovernorateDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public string GovernorateCode { get; set; }
        public bool IsActive { get; set; }
        public List<CityDto> Cities { get; set; } = new();
        public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
    }
}
