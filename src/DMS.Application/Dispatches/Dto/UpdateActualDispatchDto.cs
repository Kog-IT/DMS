using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Dispatches;
using System;

namespace DMS.Dispatches.Dto;

[AutoMapTo(typeof(ActualDispatch))]
public class UpdateActualDispatchDto : EntityDto<int>
{
    public int SalesmanId { get; set; }
    public DateTime DispatchDate { get; set; }
    public string Notes { get; set; }
    public decimal TotalAmount { get; set; }
}
