using Abp.Configuration;
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
        };
    }
}
