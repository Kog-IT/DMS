using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace DMS.Cities.Dto
{
    public class CityDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public bool IsActive { get; set; }
        public int GovernorateId { get; set; }
        public string GovernorateName { get; set; }
        public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
    }
}
