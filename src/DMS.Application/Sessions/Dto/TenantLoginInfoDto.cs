using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace DMS.Sessions.Dto;

[AutoMapFrom(typeof(Tenant))]
public class TenantLoginInfoDto : EntityDto
{
    public string TenancyName { get; set; }

    public string Name { get; set; }

    [StringLength(Tenant.MaxImageUrlLength)]
    public string? ImageUrl { get; set; }
}
