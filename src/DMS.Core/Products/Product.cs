using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DMS.Categories;

namespace DMS.Products
{
    public class Product : FullAuditedEntity<int>,IMustHaveTenant
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 2048;
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public int CategoryId { get; set; } // Foreign Key
        public virtual Category Category { get; set; }
        public int TenantId { get ; set ; }

        public virtual ICollection<ProductVariant> Variants { get; set; }
        //public virtual ICollection<ProductImage> Images { get; set; }
    }
}
