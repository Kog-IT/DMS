using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Salesmen.Dto;

[AutoMapTo(typeof(Salesman))]
public class UpdateSalesmanDto : EntityDto<int>
{
    [Required]
    [StringLength(Salesman.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Salesman.MaxJobCodeLength)]
    public string JobCode { get; set; }

    [StringLength(Salesman.MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(Salesman.MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(Salesman.MaxNationalNumberLength)]
    public string NationalNumber { get; set; }

    [StringLength(Salesman.MaxAddressLength)]
    public string Address { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? AppointmentDate { get; set; }

    [StringLength(Salesman.MaxPathLength)]
    public string Path { get; set; }

    [StringLength(Salesman.MaxSalesSupervisorIdLength)]
    public string SalesSupervisorId { get; set; }

    [StringLength(Salesman.MaxUserNameLength)]
    public string UserName { get; set; }

    public List<int> WarehouseIds { get; set; } = new();

    public bool IsActive { get; set; } = true;
}
