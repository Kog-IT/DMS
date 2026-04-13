using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Visits.Dto;

public class UploadVisitPhotoDto
{
    public int VisitId { get; set; }

    [Required]
    public string FileBase64 { get; set; }

    [Required]
    public string FileExtension { get; set; }

    public DateTime CapturedAt { get; set; }
    public string Caption { get; set; }
}
