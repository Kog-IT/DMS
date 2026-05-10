#nullable enable

using Abp.MultiTenancy;
using DMS.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace DMS.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public const int MaxImageUrlLength = 500;

    [StringLength(MaxImageUrlLength)]
    public string? ImageUrl { get; set; }

    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
