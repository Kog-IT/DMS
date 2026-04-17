using Abp.Application.Services.Dto;
using DMS.Customers;
using System;

namespace DMS.PriceLists.Dto;

public class PagedPriceListRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
    public CustomerClassification? ForClassification { get; set; }
    public DateTime? ActiveOn { get; set; }
}
