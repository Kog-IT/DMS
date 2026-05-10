using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Categories.Dto;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Media;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Categories
{
    [AbpAuthorize(PermissionNames.Pages_Categories)]
    public class CategoryAppService : DmsCrudAppService<
        Category, CategoryDto, int,
        PagedCategoryResultRequestDto,
        CreateCategoryDto, UpdateCategoryDto>, ICategoryAppService
    {
        private readonly IRepository<MediaFile, int> _mediaRepository;

        public CategoryAppService(
            IRepository<Category, int> repository,
            IRepository<MediaFile, int> mediaRepository)
            : base(repository)
        {
            GetPermissionName = PermissionNames.Pages_Categories;
            GetAllPermissionName = PermissionNames.Pages_Categories;
            CreatePermissionName = PermissionNames.Pages_Categories_Create;
            UpdatePermissionName = PermissionNames.Pages_Categories_Edit;
            DeletePermissionName = PermissionNames.Pages_Categories_Delete;

            _mediaRepository = mediaRepository;
        }

        protected override CategoryDto MapToEntityDto(Category entity)
        {
            var dto = base.MapToEntityDto(entity);
            dto.Media = _mediaRepository.GetAll()
                .Where(m => m.MediaType == MediaType.Category && m.ModelId == entity.Id)
                .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
                .ToList();
            return dto;
        }

        protected override IQueryable<Category> CreateFilteredQuery(PagedCategoryResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    c => c.Name.Contains(input.Keyword));
        }

        public async Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        [HttpPost]
        public async Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("DeletedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                await Repository.DeleteAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("DeletedSuccessfully"));
        }

        public async Task<ApiResponse<object>> BulkActivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }
    }
}
