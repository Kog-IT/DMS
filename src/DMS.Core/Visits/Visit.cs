using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DMS.Customers;
using DMS.Routes;

namespace DMS.Visits;

public class Visit : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNotesLength = 2048;
    public const int MaxSkipReasonLength = 512;
    public const int MaxNoSaleReasonLength = 512;
    public const int MaxExternalIdLength = 128;

    public int TenantId { get; set; }

    public int? RouteId { get; set; }
    public int? RouteItemId { get; set; }

    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public long AssignedUserId { get; set; }

    public VisitStatus Status { get; set; } = VisitStatus.Planned;

    public DateTime PlannedDate { get; set; }

    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }

    public int? DurationMinutes { get; set; }

    [StringLength(MaxNotesLength)]
    public string Notes { get; set; }

    [StringLength(MaxSkipReasonLength)]
    public string SkipReason { get; set; }

    [StringLength(MaxNoSaleReasonLength)]
    public string NoSaleReason { get; set; }

    /// <summary>Client-generated ID for offline sync deduplication.</summary>
    [StringLength(MaxExternalIdLength)]
    public string ExternalId { get; set; }

    public virtual ICollection<VisitPhoto> Photos { get; set; } = new List<VisitPhoto>();

    public virtual ICollection<RouteItem> RouteItems { get; set; } = new List<RouteItem>();
}
