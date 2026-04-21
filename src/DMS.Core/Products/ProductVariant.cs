using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;

namespace DMS.Products
{
    public class ProductVariant : FullAuditedEntity<int>
    {
        [Required]
        public string Capacity { get; set; } 

        [Required]
        public string SKU { get; set; } // كود فريد لكل عبوة
        public string Url { get; set; }

        public decimal Price { get; set; }
     
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
