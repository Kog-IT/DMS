using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace DMS.SalesmanRequests.Dto;

[AutoMapFrom(typeof(SalesmanRequest))]
public class SalesmanRequestDto : EntityDto<int>
{
    public int SalesmanId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime RequestDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public string RejectionReason { get; set; }
    public List<SalesmanRequestItemDto> Items { get; set; } = new();
}
