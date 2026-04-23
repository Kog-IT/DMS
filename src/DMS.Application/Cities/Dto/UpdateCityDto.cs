using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace DMS.Cities.Dto
{
    public class UpdateCityDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public bool IsActive { get; set; }
        public int GovernorateId { get; set; }

        public string GovernorateName { get; set; }
    }
}
