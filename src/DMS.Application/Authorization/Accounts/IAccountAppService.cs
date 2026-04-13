using Abp.Application.Services;
using DMS.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace DMS.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
