using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DMS.Media;

public class MediaFile : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxFilePathLength = 512;
    public const int MaxFileNameLength = 256;
    public const int MaxContentTypeLength = 128;

    public int TenantId { get; set; }

    public MediaType MediaType { get; set; }

    public int ModelId { get; set; }

    [Required]
    [StringLength(MaxFileNameLength)]
    public string FileName { get; set; }

    [Required]
    [StringLength(MaxFilePathLength)]
    public string FilePath { get; set; }

    [StringLength(MaxContentTypeLength)]
    public string ContentType { get; set; }

    public long FileSizeBytes { get; set; }
}
