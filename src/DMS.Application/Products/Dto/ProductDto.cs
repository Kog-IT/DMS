using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Companies;

namespace DMS.Products.Dto
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : FullAuditedEntityDto<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
