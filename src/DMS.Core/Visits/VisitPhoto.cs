using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Visits;

public class VisitPhoto : Entity<int>, IMustHaveTenant
{
    public const int MaxFilePathLength = 512;
    public const int MaxCaptionLength = 256;

    public int TenantId { get; set; }

    public int VisitId { get; set; }
    public virtual Visit Visit { get; set; }

    [Required]
    [StringLength(MaxFilePathLength)]
    public string FilePath { get; set; }

    public DateTime CapturedAt { get; set; }

    [StringLength(MaxCaptionLength)]
    public string Caption { get; set; }
}
