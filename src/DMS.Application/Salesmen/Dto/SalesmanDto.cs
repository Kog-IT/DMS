using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace DMS.Salesmen.Dto;

[AutoMapFrom(typeof(Salesman))]
public class SalesmanDto : EntityDto<int>
{
    public string Name { get; set; }
    public string JobCode { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string NationalNumber { get; set; }
    public string Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public string Path { get; set; }
    public string SalesSupervisorId { get; set; }
    public string UserName { get; set; }
    public long? UserId { get; set; }
    public bool IsActive { get; set; }
    public List<int> WarehouseIds { get; set; } = new();
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
