using Abp.Authorization;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;

namespace DMS.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
