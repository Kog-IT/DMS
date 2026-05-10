using Abp.Application.Services.Dto;
using DMS.Cities.Dto;
using DMS.Common.Dto;
using System.Collections.Generic;
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
        Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
        Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
        Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
        Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
        Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
    }
}
