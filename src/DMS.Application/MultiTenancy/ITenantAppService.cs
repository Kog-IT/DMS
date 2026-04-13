using Abp.Application.Services;
using DMS.MultiTenancy.Dto;

namespace DMS.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

