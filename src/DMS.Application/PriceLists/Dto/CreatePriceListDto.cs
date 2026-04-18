using Abp.AutoMapper;
using DMS.Customers;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.PriceLists.Dto;

[AutoMapTo(typeof(PriceList))]
public class CreatePriceListDto
{
    [Required]
    [StringLength(PriceList.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(PriceList.MaxDescriptionLength)]
    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public CustomerClassification? ForClassification { get; set; }
}
