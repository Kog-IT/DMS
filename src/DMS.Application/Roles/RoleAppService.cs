using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DMS.Roles;

[AbpAuthorize(PermissionNames.Pages_Roles)]
public class RoleAppService : DmsCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
{
    private readonly RoleManager _roleManager;
    private readonly UserManager _userManager;

    public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager)
        : base(repository)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public override async Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto input)
    {
        var role = ObjectMapper.Map<Role>(input);
        role.SetNormalizedName();

        CheckErrors(await _roleManager.CreateAsync(role));

        var grantedPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => input.GrantedPermissions.Contains(p.Name))
            .ToList();

        await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        var dto = MapToEntityDto(role);
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<ListResultDto<RoleListDto>>> GetRolesAsync(GetRolesInput input)
    {
        var roles = await _roleManager
            .Roles
            .WhereIf(
                !input.Permission.IsNullOrWhiteSpace(),
                r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
            )
            .ToListAsync();

        var result = new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        return Ok(result, L("RetrievedSuccessfully"));
    }

    public override async Task<ApiResponse<RoleDto>> UpdateAsync(RoleDto input)
    {
        var role = await _roleManager.GetRoleByIdAsync(input.Id);

        ObjectMapper.Map(input, role);

        CheckErrors(await _roleManager.UpdateAsync(role));

        var grantedPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => input.GrantedPermissions.Contains(p.Name))
            .ToList();

        await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        var dto = MapToEntityDto(role);
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public override async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var role = await _roleManager.FindByIdAsync(input.Id.ToString());
        var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

        foreach (var user in users)
        {
            CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
        }

        CheckErrors(await _roleManager.DeleteAsync(role));
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    public Task<ApiResponse<ListResultDto<PermissionDto>>> GetAllPermissions()
    {
        var permissions = PermissionManager.GetAllPermissions();

        var result = new ListResultDto<PermissionDto>(
            ObjectMapper.Map<List<PermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList()
        );
        return Task.FromResult(Ok(result, L("RetrievedSuccessfully")));
    }

    protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
    {
        return Repository.GetAllIncluding(x => x.Permissions)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
            || x.DisplayName.Contains(input.Keyword)
            || x.Description.Contains(input.Keyword));
    }

    protected override async Task<Role> GetEntityByIdAsync(int id)
    {
        return await Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
    }

    protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    public async Task<ApiResponse<GetRoleForEditOutput>> GetRoleForEdit(EntityDto input)
    {
        var permissions = PermissionManager.GetAllPermissions();
        var role = await _roleManager.GetRoleByIdAsync(input.Id);
        var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
        var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

        var result = new GetRoleForEditOutput
        {
            Role = roleEditDto,
            Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
            GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
        };
        return Ok(result, L("RetrievedSuccessfully"));
    }
}
