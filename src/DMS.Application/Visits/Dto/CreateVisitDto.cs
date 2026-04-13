using Abp.AutoMapper;
using DMS.Visits;
using System;

namespace DMS.Visits.Dto;

[AutoMapTo(typeof(Visit))]
public class CreateVisitDto
{
    public int? RouteId { get; set; }
    public int? RouteItemId { get; set; }
    public int CustomerId { get; set; }
    public long AssignedUserId { get; set; }
    public DateTime PlannedDate { get; set; }
    public string ExternalId { get; set; }
}
