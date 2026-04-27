using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Dispatches;
using System;

namespace DMS.Dispatches.Dto;

[AutoMapFrom(typeof(PlannedDispatch))]
public class PlannedDispatchDto : EntityDto<int>
{
    public int SalesmanId { get; set; }
    public DateTime DispatchDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public bool IsActive { get; set; }
}
