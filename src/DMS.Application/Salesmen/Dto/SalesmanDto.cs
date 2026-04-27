using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.Salesmen.Dto;

[AutoMapFrom(typeof(Salesman))]
public class SalesmanDto : EntityDto<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Mobile { get; set; }
    public string NationalId { get; set; }
    public int GovernorateId { get; set; }
    public int CityId { get; set; }
    public int? AssignedWarehouseId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
