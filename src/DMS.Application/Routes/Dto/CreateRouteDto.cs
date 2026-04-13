using Abp.AutoMapper;
using DMS.Routes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Routes.Dto;

[AutoMapTo(typeof(Route))]
public class CreateRouteDto
{
    [Required]
    [StringLength(Route.MaxNameLength)]
    public string Name { get; set; }

    public long AssignedUserId { get; set; }

    public DateTime PlannedDate { get; set; }

    [StringLength(Route.MaxNotesLength)]
    public string Notes { get; set; }

    public List<CreateRouteItemDto> Items { get; set; } = new();
}
