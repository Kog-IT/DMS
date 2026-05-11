using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.SalesmanRequests.Dto;

[AutoMapTo(typeof(SalesmanRequest))]
public class CreateSalesmanRequestDto
{
    [Required]
    public int SalesmanId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    public DateTime RequestDate { get; set; }

    public string Notes { get; set; }

    public List<CreateSalesmanRequestItemDto> Items { get; set; } = new();
}
