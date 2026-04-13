using Abp.Application.Services.Dto;
using DMS.Visits;
using System;

namespace DMS.Visits.Dto;

public class PagedVisitResultRequestDto : PagedResultRequestDto
{
    public VisitStatus? Status { get; set; }
    public long? AssignedUserId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
