using Abp.Application.Services.Dto;
using DMS.Categories.Dto;
using DMS.Common.Dto;
using System.Threading.Tasks;

namespace DMS.Categories;

public interface ICategoryAppService
{
    Task<ApiResponse<CategoryDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<CategoryDto>>> GetAllAsync(PagedCategoryResultRequestDto input);
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto input);
    Task<ApiResponse<CategoryDto>> UpdateAsync(UpdateCategoryDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
