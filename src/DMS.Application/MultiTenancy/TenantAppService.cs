using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using DMS.Authorization;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Editions;
using DMS.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DMS.MultiTenancy;

[AbpAuthorize(PermissionNames.Pages_Tenants)]
public class TenantAppService : DmsCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
{
    private readonly TenantManager _tenantManager;
    private readonly EditionManager _editionManager;
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;

    public TenantAppService(
        IRepository<Tenant, int> repository,
        TenantManager tenantManager,
        EditionManager editionManager,
        UserManager userManager,
        RoleManager roleManager,
        IAbpZeroDbMigrator abpZeroDbMigrator)
        : base(repository)
    {
        _tenantManager = tenantManager;
        _editionManager = editionManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _abpZeroDbMigrator = abpZeroDbMigrator;
    }

    public override async Task<ApiResponse<TenantDto>> CreateAsync(CreateTenantDto input)
    {
        var tenant = ObjectMapper.Map<Tenant>(input);
        tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
            ? null
            : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

        var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
        if (defaultEdition != null)
        {
            tenant.EditionId = defaultEdition.Id;
        }

        await _tenantManager.CreateAsync(tenant);
        await CurrentUnitOfWork.SaveChangesAsync();

        _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

        using (CurrentUnitOfWork.SetTenantId(tenant.Id))
        {
            CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
            await CurrentUnitOfWork.SaveChangesAsync();

            var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
            await _roleManager.GrantAllPermissionsAsync(adminRole);

            var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
            await _userManager.InitializeOptionsAsync(tenant.Id);
            CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
            await CurrentUnitOfWork.SaveChangesAsync();

            CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return Ok(MapToEntityDto(tenant), L("CreatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public async Task<ApiResponse<TenantDto>> UpdateTenantImageAsync(UpdateTenantImageDto input)
    {
        var tenant = await _tenantManager.GetByIdAsync(input.Id);
        tenant.ImageUrl = input.ImageUrl;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok(MapToEntityDto(tenant), L("UpdatedSuccessfully"));
    }

    protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
    }

    protected override IQueryable<Tenant> ApplySorting(IQueryable<Tenant> query, PagedTenantResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected override void MapToEntity(TenantDto updateInput, Tenant entity)
    {
        entity.Name = updateInput.Name;
        entity.TenancyName = updateInput.TenancyName;
        entity.IsActive = updateInput.IsActive;
        entity.ImageUrl = updateInput.ImageUrl;
    }

    public override async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var tenant = await _tenantManager.GetByIdAsync(input.Id);
        await _tenantManager.DeleteAsync(tenant);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }
}
