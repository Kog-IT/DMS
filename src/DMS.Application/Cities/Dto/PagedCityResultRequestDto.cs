using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace DMS.Cities.Dto
{
    public class PagedCityResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? GovernorateId { get; set; }
    }
}
