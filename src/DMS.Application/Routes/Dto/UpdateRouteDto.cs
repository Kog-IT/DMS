using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Routes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Routes.Dto;

[AutoMapTo(typeof(Route))]
public class UpdateRouteDto : EntityDto<int>
{
    [Required]
    [StringLength(Route.MaxNameLength)]
    public string Name { get; set; }

    public long AssignedUserId { get; set; }

    public DateTime PlannedDate { get; set; }

    public RouteStatus Status { get; set; }

    [StringLength(Route.MaxNotesLength)]
    public string Notes { get; set; }

    public List<UpdateRouteItemDto> Items { get; set; } = new();
}
