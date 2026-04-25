using Abp.Application.Services.Dto;
using DMS.Cities.Dto;
using DMS.Common.Dto;
using System.Threading.Tasks;

namespace DMS.Cities
{
    public interface ICityAppService
    {
        Task<ApiResponse<CityDto>> GetAsync(EntityDto<int> input);
        Task<ApiResponse<PagedResultDto<CityDto>>> GetAllAsync(PagedCityResultRequestDto input);
        Task<ApiResponse<CityDto>> CreateAsync(CreateCityDto input);
        Task<ApiResponse<CityDto>> UpdateAsync(UpdateCityDto input);
        Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    }
}
