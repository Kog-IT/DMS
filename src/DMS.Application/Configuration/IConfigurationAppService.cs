using DMS.Configuration.Dto;
using System.Threading.Tasks;

namespace DMS.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
