using Abp.Application.Services;
using DMS.Categories.Dto;

namespace DMS.Categories;

public interface ICategoryAppService : IAsyncCrudAppService<
    CategoryDto,
    int,
    PagedCategoryResultRequestDto,
    CreateCategoryDto,
    UpdateCategoryDto>
{
}
