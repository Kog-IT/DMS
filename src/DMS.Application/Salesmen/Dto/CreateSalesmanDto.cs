using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Salesmen.Dto;

[AutoMapTo(typeof(Salesman))]
public class CreateSalesmanDto
{
    [Required]
    [StringLength(Salesman.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Salesman.MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(Salesman.MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(Salesman.MaxNationalIdLength)]
    public string NationalId { get; set; }

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    public int? AssignedWarehouseId { get; set; }

    [StringLength(Salesman.MaxImageUrlLength)]
    public string ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
