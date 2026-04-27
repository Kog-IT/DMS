using Abp.AutoMapper;
using DMS.Dispatches;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Dispatches.Dto;

[AutoMapTo(typeof(ActualDispatch))]
public class CreateActualDispatchDto
{
    [Required]
    public int SalesmanId { get; set; }

    [Required]
    public DateTime DispatchDate { get; set; }

    public string Notes { get; set; }

    public decimal TotalAmount { get; set; } = 0;
}
