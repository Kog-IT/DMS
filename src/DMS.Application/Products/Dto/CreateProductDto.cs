using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Products.Dto
{
    [AutoMapTo(typeof(Product))]
    public class CreateProductDto
    {
        [Required]
        [StringLength(Product.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(Product.MaxDescriptionLength)]
        public string Description { get; set; }

        [StringLength(Product.MaxCodeLength)]
        public string Code { get; set; }

        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal TaxRate { get; set; }

        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal VipClientsPrice { get; set; }

        public int NumOfUnitPerCartoon { get; set; }
        public int SmallerUnitsOfMeasurements { get; set; }
        public int SafetyStocks { get; set; }
        public decimal PackSize { get; set; }
        public decimal WeightPerKG { get; set; }
        public decimal NetWeightPerKG { get; set; }
        public decimal TotalPackSize { get; set; }
        public decimal TotalWeightPerKG { get; set; }
        public decimal TotalNetWeightPerKG { get; set; }

        public int Unit { get; set; }
        public int Grade { get; set; }
        public int ProductStatus { get; set; }

        [StringLength(Product.MaxProductAPILength)]
        public string ProductAPI { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int CategoryId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        public List<string> Paths { get; set; } = new List<string>();
    }
}
