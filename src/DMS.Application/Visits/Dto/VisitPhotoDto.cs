using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Visits;
using System;

namespace DMS.Visits.Dto;

[AutoMapFrom(typeof(VisitPhoto))]
public class VisitPhotoDto : EntityDto<int>
{
    public int VisitId { get; set; }
    public string FilePath { get; set; }
    public DateTime CapturedAt { get; set; }
    public string Caption { get; set; }
}
