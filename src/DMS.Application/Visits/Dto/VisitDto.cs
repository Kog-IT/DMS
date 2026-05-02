using Abp.Application.Services.Dto;
using DMS.Visits;
using System;
using System.Collections.Generic;

namespace DMS.Visits.Dto;

public class VisitDto : FullAuditedEntityDto<int>
{
    public int? RouteId { get; set; }
    public int? RouteItemId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public long AssignedUserId { get; set; }
    public VisitStatus Status { get; set; }
    public string StatusName { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }
    public int? DurationMinutes { get; set; }
    public string Notes { get; set; }
    public string SkipReason { get; set; }
    public string NoSaleReason { get; set; }
    public string ExternalId { get; set; }
    public List<VisitPhotoDto> Photos { get; set; } = new();
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
