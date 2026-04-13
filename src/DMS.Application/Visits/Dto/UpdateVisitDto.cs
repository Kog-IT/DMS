using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Visits;
using System;

namespace DMS.Visits.Dto;

[AutoMapTo(typeof(Visit))]
public class UpdateVisitDto : EntityDto<int>
{
    public int CustomerId { get; set; }
    public long AssignedUserId { get; set; }
    public DateTime PlannedDate { get; set; }
    public string Notes { get; set; }
}
