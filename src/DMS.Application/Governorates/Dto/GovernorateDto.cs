using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.Governorates.Dto
{
    [AutoMapFrom(typeof(Governorate))]
    public class GovernorateDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public string GovernorateCode { get; set; }
        public bool IsActive { get; set; }
    }
}
