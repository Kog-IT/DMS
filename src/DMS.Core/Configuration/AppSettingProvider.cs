using Abp.Configuration;
using DMS.Customers;
using DMS.Invoices;
using DMS.Orders;
using DMS.Payments;
using DMS.Visits;
using System.Collections.Generic;

namespace DMS.Configuration;

public class AppSettingProvider : SettingProvider
{
    public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
    {
        return new[]
        {
            new SettingDefinition(AppSettingNames.UiTheme, "red",
                scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),

            new SettingDefinition(VisitSettingNames.GeofencingEnabled, "true",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(VisitSettingNames.GeofencingRadiusMeters, "200",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(VisitSettingNames.GpsEnforcement, GpsEnforcementMode.Warn,
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(VisitSettingNames.DefaultVisitDurationMinutes, "30",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(VisitSettingNames.AverageTravelSpeedKmh, "30",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ClassificationSettingNames.LookbackDays, "90",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ClassificationSettingNames.ThresholdA, "8",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ClassificationSettingNames.ThresholdB, "4",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ClassificationSettingNames.MinCustomersForPercentile, "10",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ClassificationSettingNames.ScheduleFrequency, "Weekly",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(OrderSettingNames.AllowOrdersWithoutStock, "true",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(OrderSettingNames.DiscountLimitSalesRep, "0",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(OrderSettingNames.DiscountLimitSupervisor, "0",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(InvoiceSettingNames.AutoGenerateTrigger, "Manual",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(InvoiceSettingNames.NumberPrefix, "INV",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(InvoiceSettingNames.NextSequenceNumber, "1",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(InvoiceSettingNames.DueDaysDefault, "30",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ReceiptSettingNames.NextSequenceNumber, "1",
                scopes: SettingScopes.Application | SettingScopes.Tenant),

            new SettingDefinition(ReceiptSettingNames.CurrentYear, "0",
                scopes: SettingScopes.Application | SettingScopes.Tenant),
        };
    }
}
