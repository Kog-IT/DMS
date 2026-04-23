using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Governorates.Dto;
using System.Threading.Tasks;

namespace DMS.Governorates
{
    public interface IGovernorateAppService
    {
        Task<ApiResponse<GovernorateDto>> GetAsync(EntityDto<int> input);
        Task<ApiResponse<PagedResultDto<GovernorateDto>>> GetAllAsync(PagedGovernorateResultRequestDto input);
        Task<ApiResponse<GovernorateDto>> CreateAsync(CreateGovernorateDto input);
        Task<ApiResponse<GovernorateDto>> UpdateAsync(UpdateGovernorateDto input);
        Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    }
}
