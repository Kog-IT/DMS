using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace DMS.Products.Dto
{
    public class PagedProductResultRequestDto : PagedAndSortedResultRequestDto
    {
       
        public string Keyword { get; set; }

       
        public int? CategoryId { get; set; }
    }
}
