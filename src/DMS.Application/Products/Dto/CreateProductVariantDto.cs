using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Products.Dto
{
    public class CreateProductVariantDto
    {
        public string Capacity { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
    }
}
