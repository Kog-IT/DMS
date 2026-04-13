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

        var customers = context.CreatePermission(PermissionNames.Pages_Customers, L("Customers"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Create, L("CustomersCreate"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Edit, L("CustomersEdit"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Delete, L("CustomersDelete"));

        var routes = context.CreatePermission(PermissionNames.Pages_Routes, L("Routes"));
        routes.CreateChildPermission(PermissionNames.Pages_Routes_Create, L("RoutesCreate"));
        routes.CreateChildPermission(PermissionNames.Pages_Routes_Edit, L("RoutesEdit"));
        routes.CreateChildPermission(PermissionNames.Pages_Routes_Delete, L("RoutesDelete"));

        var visits = context.CreatePermission(PermissionNames.Pages_Visits, L("Visits"));
        visits.CreateChildPermission(PermissionNames.Pages_Visits_Create, L("VisitsCreate"));
        visits.CreateChildPermission(PermissionNames.Pages_Visits_Edit, L("VisitsEdit"));
        visits.CreateChildPermission(PermissionNames.Pages_Visits_Delete, L("VisitsDelete"));
        visits.CreateChildPermission(PermissionNames.Pages_Visits_CheckIn, L("VisitsCheckIn"));
        visits.CreateChildPermission(PermissionNames.Pages_Visits_CheckOut, L("VisitsCheckOut"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, DMSConsts.LocalizationSourceName);
    }
}
