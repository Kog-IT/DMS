using Abp.Application.Services;
using DMS.Sessions.Dto;
using System.Threading.Tasks;

namespace DMS.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
