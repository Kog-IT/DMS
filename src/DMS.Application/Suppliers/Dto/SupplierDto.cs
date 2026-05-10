using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;

namespace DMS.Suppliers.Dto;

[AutoMapFrom(typeof(Supplier))]
public class SupplierDto : EntityDto<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public int GovernorateId { get; set; }
    public int CityId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<DMS.Application.Media.Dto.MediaItemDto> Media { get; set; } = new List<DMS.Application.Media.Dto.MediaItemDto>();
}
