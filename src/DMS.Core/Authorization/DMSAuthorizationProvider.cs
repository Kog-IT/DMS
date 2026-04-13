using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace DMS.Authorization;

public class DMSAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

        var companies = context.CreatePermission(PermissionNames.Pages_Companies, L("Companies"));
        companies.CreateChildPermission(PermissionNames.Pages_Companies_Create, L("CompaniesCreate"));
        companies.CreateChildPermission(PermissionNames.Pages_Companies_Edit, L("CompaniesEdit"));
        companies.CreateChildPermission(PermissionNames.Pages_Companies_Delete, L("CompaniesDelete"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, DMSConsts.LocalizationSourceName);
    }
}
