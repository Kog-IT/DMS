using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.Products.Dto
{
    [AutoMapTo(typeof(Product))]
    public class UpdateProductDto :  EntityDto<int>
    {
       
        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } 

        [MaxLength(50)]
        public string Barcode { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
