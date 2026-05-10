using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Roles.Dto;
using DMS.Users.Dto;
using System.Threading.Tasks;

namespace DMS.Users;

public interface IUserAppService
{
    Task<ApiResponse<UserDto>> GetAsync(EntityDto<long> input);
    Task<ApiResponse<PagedResultDto<UserDto>>> GetAllAsync(PagedUserResultRequestDto input);
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto input);
    Task<ApiResponse<UserDto>> UpdateAsync(UserDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<long> input);

    Task<ApiResponse<object>> DeActivate(EntityDto<long> user);
    Task<ApiResponse<object>> Activate(EntityDto<long> user);
    Task<ApiResponse<ListResultDto<RoleDto>>> GetRoles();
    Task<ApiResponse<object>> ChangeLanguage(ChangeUserLanguageDto input);
    Task<ApiResponse<bool>> ChangePassword(ChangePasswordDto input);
}
