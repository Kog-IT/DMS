using Abp.Application.Services.Dto;
using DMS.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace DMS.MultiTenancy.Dto;

public class UpdateTenantImageDto : EntityDto
{
    [StringLength(Tenant.MaxImageUrlLength)]
    public string? ImageUrl { get; set; }
}
