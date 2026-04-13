using Abp.Authorization;
using Abp.Runtime.Session;
using DMS.Configuration.Dto;
using System.Threading.Tasks;

namespace DMS.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : DMSAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
