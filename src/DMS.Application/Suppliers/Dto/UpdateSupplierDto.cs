using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace DMS.Suppliers.Dto;

[AutoMapTo(typeof(Supplier))]
public class UpdateSupplierDto : EntityDto<int>
{
    [Required]
    [StringLength(Supplier.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(Supplier.MaxCodeLength)]
    public string Code { get; set; }

    [StringLength(Supplier.MaxMobileLength)]
    public string Mobile { get; set; }

    [StringLength(Supplier.MaxEmailLength)]
    public string Email { get; set; }

    [StringLength(Supplier.MaxAddressLength)]
    public string Address { get; set; }

    [StringLength(Supplier.MaxCollaborationAdministratorLength)]
    public string CollaborationAdministrator { get; set; }

    public int GovernorateId { get; set; }

    public int CityId { get; set; }

    [StringLength(Supplier.MaxPathLength)]
    public string Path { get; set; }

    public bool IsActive { get; set; }
}
