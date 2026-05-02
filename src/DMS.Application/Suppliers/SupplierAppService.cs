using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Suppliers.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Suppliers;

public class SupplierAppService : DmsCrudAppService<
    Supplier, SupplierDto, int,
    PagedSupplierResultRequestDto,
    CreateSupplierDto, UpdateSupplierDto>, ISupplierAppService
{
    private readonly IRepository<DMS.Media.MediaFile, int> _mediaRepository;

    public SupplierAppService(
        IRepository<Supplier, int> repository,
        IRepository<DMS.Media.MediaFile, int> mediaRepository)
        : base(repository)
    {
        _mediaRepository = mediaRepository;
    }

    protected override SupplierDto MapToEntityDto(Supplier entity)
    {
        var dto = base.MapToEntityDto(entity);
        dto.Media = _mediaRepository.GetAll()
            .Where(m => m.MediaType == DMS.Media.MediaType.Supplier && m.ModelId == entity.Id)
            .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
            .ToList();
        return dto;
    }

    protected override IQueryable<Supplier> CreateFilteredQuery(PagedSupplierResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Keyword) || x.Code.Contains(input.Keyword))
            .WhereIf(input.GovernorateId.HasValue, x => x.GovernorateId == input.GovernorateId.Value)
            .WhereIf(input.CityId.HasValue, x => x.CityId == input.CityId.Value)
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
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
