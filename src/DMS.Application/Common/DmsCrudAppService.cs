using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Common;

public abstract class DmsCrudAppService<
    TEntity,
    TEntityDto,
    TPrimaryKey,
    TGetAllInput,
    TCreateInput,
    TUpdateInput>
    : DMSAppServiceBase
    where TEntity : class, IEntity<TPrimaryKey>
    where TEntityDto : IEntityDto<TPrimaryKey>
    where TUpdateInput : IEntityDto<TPrimaryKey>
    where TGetAllInput : PagedResultRequestDto
{
    protected IRepository<TEntity, TPrimaryKey> Repository { get; }
    public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

    protected string GetPermissionName { get; set; }
    protected string GetAllPermissionName { get; set; }
    protected string CreatePermissionName { get; set; }
    protected string UpdatePermissionName { get; set; }
    protected string DeletePermissionName { get; set; }

    protected DmsCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
    {
        Repository = repository;
        AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
    }

    public virtual async Task<ApiResponse<TEntityDto>> GetAsync(EntityDto<TPrimaryKey> input)
    {
        CheckPermission(GetPermissionName);
        var entity = await GetEntityByIdAsync(input.Id);
        var dto = MapToEntityDto(entity);
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public virtual async Task<ApiResponse<PagedResultDto<TEntityDto>>> GetAllAsync(TGetAllInput input)
    {
        CheckPermission(GetAllPermissionName);
        var query = CreateFilteredQuery(input);
        var totalCount = await AsyncQueryableExecuter.CountAsync(query);
        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);
        var entities = await AsyncQueryableExecuter.ToListAsync(query);
        var dtos = entities.Select(MapToEntityDto).ToList();
        var paged = new PagedResultDto<TEntityDto>(totalCount, dtos);
        return Ok(paged, L("RetrievedSuccessfully"));
    }

    public virtual async Task<ApiResponse<TEntityDto>> CreateAsync(TCreateInput input)
    {
        CheckPermission(CreatePermissionName);
        var entity = MapToEntity(input);
        await Repository.InsertAsync(entity);
        await CurrentUnitOfWork.SaveChangesAsync();
        var dto = MapToEntityDto(entity);
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public virtual async Task<ApiResponse<TEntityDto>> UpdateAsync(TUpdateInput input)
    {
        CheckPermission(UpdatePermissionName);
        var entity = await Repository.GetAsync(input.Id);
        MapToEntity(input, entity);
        await CurrentUnitOfWork.SaveChangesAsync();
        var dto = MapToEntityDto(entity);
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public virtual async Task<ApiResponse<object>> DeleteAsync(EntityDto<TPrimaryKey> input)
    {
        CheckPermission(DeletePermissionName);
        await Repository.DeleteAsync(input.Id);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    protected virtual async Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        => await Repository.GetAsync(id);

    protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetAllInput input)
        => Repository.GetAll();

    protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TGetAllInput input)
    {
        var sorting = (input as PagedAndSortedResultRequestDto)?.Sorting;
        return sorting != null ? query.OrderBy(e => sorting) : query;
    }

    protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TGetAllInput input)
        => query.PageBy(input);

    protected virtual TEntityDto MapToEntityDto(TEntity entity)
        => ObjectMapper.Map<TEntityDto>(entity);

    protected virtual TEntity MapToEntity(TCreateInput input)
        => ObjectMapper.Map<TEntity>(input);

    protected virtual void MapToEntity(TUpdateInput input, TEntity entity)
        => ObjectMapper.Map(input, entity);

    private void CheckPermission(string permissionName)
    {
        if (!string.IsNullOrEmpty(permissionName))
        {
            var isGranted = PermissionChecker.IsGranted(permissionName);
            if (!isGranted)
                throw new AbpAuthorizationException($"You are not authorized to perform this action. Required permission: {permissionName}");
        }
    }
}
