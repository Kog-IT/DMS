using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Roles.Dto;
using System.Threading.Tasks;

namespace DMS.Roles;

public interface IRoleAppService
{
    Task<ApiResponse<RoleDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<RoleDto>>> GetAllAsync(PagedRoleResultRequestDto input);
    Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto input);
    Task<ApiResponse<RoleDto>> UpdateAsync(RoleDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);

    Task<ApiResponse<ListResultDto<PermissionDto>>> GetAllPermissions();
    Task<ApiResponse<GetRoleForEditOutput>> GetRoleForEdit(EntityDto input);
    Task<ApiResponse<ListResultDto<RoleListDto>>> GetRolesAsync(GetRolesInput input);
}
