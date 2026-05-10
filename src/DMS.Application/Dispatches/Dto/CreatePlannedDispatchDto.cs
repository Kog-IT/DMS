using Abp.AutoMapper;
using DMS.Dispatches;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Dispatches.Dto;

[AutoMapTo(typeof(PlannedDispatch))]
public class CreatePlannedDispatchDto
{
    [Required]
    public int SalesmanId { get; set; }

    [Required]
    public DateTime DispatchDate { get; set; }

    public string Notes { get; set; }

    public int Status { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}
