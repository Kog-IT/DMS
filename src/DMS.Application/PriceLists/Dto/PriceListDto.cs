using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;
using System;

namespace DMS.PriceLists.Dto;

[AutoMapFrom(typeof(PriceList))]
public class PriceListDto : FullAuditedEntityDto<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public CustomerClassification? ForClassification { get; set; }
}
