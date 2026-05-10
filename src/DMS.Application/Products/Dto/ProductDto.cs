using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace DMS.Products.Dto
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : FullAuditedEntityDto<int>
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public decimal Price { get; set; }
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
        public string ProductAPI { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }

        public int? BrandId { get; set; }
        public string BrandName { get; set; }

        public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
    }
}
