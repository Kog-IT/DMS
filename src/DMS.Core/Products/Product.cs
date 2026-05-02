using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DMS.Brands;
using DMS.Categories;
using DMS.ProductGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Products
{
    public class Product : FullAuditedEntity<int>, IMustHaveTenant
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 2048;
        public const int MaxCodeLength = 50;
        public const int MaxProductAPILength = 100;

        public int TenantId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        // Pricing
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal VipClientsPrice { get; set; }

        // Packaging & Weight
        public int NumOfUnitPerCartoon { get; set; }
        public int SmallerUnitsOfMeasurements { get; set; }
        public int SafetyStocks { get; set; }
        public decimal PackSize { get; set; }
        public decimal WeightPerKG { get; set; }
        public decimal NetWeightPerKG { get; set; }
        public decimal TotalPackSize { get; set; }
        public decimal TotalWeightPerKG { get; set; }
        public decimal TotalNetWeightPerKG { get; set; }

        // Classification
        public int Unit { get; set; }
        public int Grade { get; set; }
        public int ProductStatus { get; set; }

        [StringLength(MaxProductAPILength)]
        public string ProductAPI { get; set; }

        public DateTime? ExpiryDate { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }

        public int? BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}
