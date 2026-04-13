using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Routes;

public class Route : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 256;
    public const int MaxNotesLength = 1024;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    public long AssignedUserId { get; set; }

    public DateTime PlannedDate { get; set; }

    public RouteStatus Status { get; set; } = RouteStatus.Draft;

    [StringLength(MaxNotesLength)]
    public string Notes { get; set; }

    public virtual ICollection<RouteItem> Items { get; set; } = new List<RouteItem>();
}
