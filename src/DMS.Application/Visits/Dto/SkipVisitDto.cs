using System.ComponentModel.DataAnnotations;

namespace DMS.Visits.Dto;

public class SkipVisitDto
{
    public int VisitId { get; set; }

    [Required]
    public string SkipReason { get; set; }
}
