using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Routes;
using System;
using System.Collections.Generic;

namespace DMS.Routes.Dto;

[AutoMapFrom(typeof(Route))]
public class RouteDto : FullAuditedEntityDto<int>
{
    public string Name { get; set; }
    public long AssignedUserId { get; set; }
    public DateTime PlannedDate { get; set; }
    public RouteStatus Status { get; set; }
    public string Notes { get; set; }
    public List<RouteItemDto> Items { get; set; } = new();
}
