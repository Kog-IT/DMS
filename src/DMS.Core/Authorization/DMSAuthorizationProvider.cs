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
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Classify, L("CustomersClassify"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Block, L("CustomersBlock"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_ManageCredit, L("CustomersManageCredit"));
        var contacts = customers.CreateChildPermission(PermissionNames.Pages_Customers_Contacts, L("CustomerContacts"));
        contacts.CreateChildPermission(PermissionNames.Pages_Customers_Contacts_Create, L("CustomerContactsCreate"));
        contacts.CreateChildPermission(PermissionNames.Pages_Customers_Contacts_Edit, L("CustomerContactsEdit"));
        contacts.CreateChildPermission(PermissionNames.Pages_Customers_Contacts_Delete, L("CustomerContactsDelete"));

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


        var categories = context.CreatePermission(PermissionNames.Pages_Categories, L("Categories"));
        categories.CreateChildPermission(PermissionNames.Pages_Categories_Create, L("CategoriesCreate"));
        categories.CreateChildPermission(PermissionNames.Pages_Categories_Edit, L("CategoriesEdit"));
        categories.CreateChildPermission(PermissionNames.Pages_Categories_Delete, L("CategoriesDelete"));

        var products = context.CreatePermission(PermissionNames.Pages_Products, L("Products"));
        products.CreateChildPermission(PermissionNames.Pages_Products_Create, L("CreateProduct"));
        products.CreateChildPermission(PermissionNames.Pages_Products_Edit, L("EditProduct"));
        products.CreateChildPermission(PermissionNames.Pages_Products_Delete, L("DeleteProduct"));

        var orders = context.CreatePermission(PermissionNames.Pages_Orders, L("Orders"));
        orders.CreateChildPermission(PermissionNames.Pages_Orders_Create, L("OrdersCreate"));
        orders.CreateChildPermission(PermissionNames.Pages_Orders_Edit, L("OrdersEdit"));
        orders.CreateChildPermission(PermissionNames.Pages_Orders_Delete, L("OrdersDelete"));
        orders.CreateChildPermission(PermissionNames.Pages_Orders_Approve, L("OrdersApprove"));

        var invoices = context.CreatePermission(PermissionNames.Pages_Invoices, L("Invoices"));
        invoices.CreateChildPermission(PermissionNames.Pages_Invoices_Create, L("InvoicesCreate"));
        invoices.CreateChildPermission(PermissionNames.Pages_Invoices_Edit, L("InvoicesEdit"));
        invoices.CreateChildPermission(PermissionNames.Pages_Invoices_Delete, L("InvoicesDelete"));
        invoices.CreateChildPermission(PermissionNames.Pages_Invoices_Void, L("InvoicesVoid"));

        var paymentMethods = context.CreatePermission(PermissionNames.Pages_PaymentMethods, L("PaymentMethods"));
        paymentMethods.CreateChildPermission(PermissionNames.Pages_PaymentMethods_Create, L("PaymentMethodsCreate"));
        paymentMethods.CreateChildPermission(PermissionNames.Pages_PaymentMethods_Edit, L("PaymentMethodsEdit"));
        paymentMethods.CreateChildPermission(PermissionNames.Pages_PaymentMethods_Delete, L("PaymentMethodsDelete"));

        var payments = context.CreatePermission(PermissionNames.Pages_Payments, L("Payments"));
        payments.CreateChildPermission(PermissionNames.Pages_Payments_Create, L("PaymentsCreate"));
        payments.CreateChildPermission(PermissionNames.Pages_Payments_GetReceipt, L("PaymentsGetReceipt"));
        payments.CreateChildPermission(PermissionNames.Pages_Payments_RegenerateReceipt, L("PaymentsRegenerateReceipt"));

        var priceLists = context.CreatePermission(PermissionNames.Pages_PriceLists, L("PriceLists"));
        priceLists.CreateChildPermission(PermissionNames.Pages_PriceLists_Create, L("PriceListsCreate"));
        priceLists.CreateChildPermission(PermissionNames.Pages_PriceLists_Edit, L("PriceListsEdit"));
        priceLists.CreateChildPermission(PermissionNames.Pages_PriceLists_Delete, L("PriceListsDelete"));
        priceLists.CreateChildPermission(PermissionNames.Pages_PriceLists_Assign, L("PriceListsAssign"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, DMSConsts.LocalizationSourceName);
    }
}
