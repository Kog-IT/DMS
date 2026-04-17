using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Categories.Dto;
using System.Linq;

namespace DMS.Categories;

[AbpAuthorize(PermissionNames.Pages_Categories)]
public class CategoryAppService : AsyncCrudAppService<
    Category,
    CategoryDto,
    int,
    PagedCategoryResultRequestDto,
    CreateCategoryDto,
    UpdateCategoryDto>, ICategoryAppService
{
    public CategoryAppService(IRepository<Category, int> repository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Categories;
        GetAllPermissionName = PermissionNames.Pages_Categories;
        CreatePermissionName = PermissionNames.Pages_Categories_Create;
        UpdatePermissionName = PermissionNames.Pages_Categories_Edit;
        DeletePermissionName = PermissionNames.Pages_Categories_Delete;
    }

    protected override IQueryable<Category> CreateFilteredQuery(PagedCategoryResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                c => c.Name.Contains(input.Keyword));
    }
}
