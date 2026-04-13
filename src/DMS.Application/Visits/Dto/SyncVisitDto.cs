using DMS.Visits;
using System;
using System.Collections.Generic;

namespace DMS.Visits.Dto;

public class SyncVisitDto
{
    public string ExternalId { get; set; }
    public int CustomerId { get; set; }
    public long AssignedUserId { get; set; }
    public int? RouteId { get; set; }
    public VisitStatus Status { get; set; }
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
    public List<SyncVisitPhotoDto> Photos { get; set; } = new();
}

public class SyncVisitPhotoDto
{
    public string FileBase64 { get; set; }
    public string FileExtension { get; set; }
    public DateTime CapturedAt { get; set; }
    public string Caption { get; set; }
}
