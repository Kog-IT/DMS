using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace DMS.Products.Dto
{
    public class ProductVariantDto : EntityDto<int>
    {
        public string Capacity { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Url { get; set; }


    }
}
