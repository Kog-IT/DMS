// src/DMS.Application/Warehouses/WarehouseAppService.cs
using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Warehouses.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Warehouses;

public class WarehouseAppService : DmsCrudAppService<
    Warehouse, WarehouseDto, int,
    PagedWarehouseResultRequestDto,
    CreateWarehouseDto, UpdateWarehouseDto>, IWarehouseAppService
{
    private readonly IRepository<WarehouseProduct, int> _warehouseProductRepository;
    private readonly IRepository<DMS.Media.MediaFile, int> _mediaRepository;

    public WarehouseAppService(
        IRepository<Warehouse, int> repository,
        IRepository<WarehouseProduct, int> warehouseProductRepository,
        IRepository<DMS.Media.MediaFile, int> mediaRepository)
        : base(repository)
    {
        _warehouseProductRepository = warehouseProductRepository;
        _mediaRepository = mediaRepository;
    }

    protected override WarehouseDto MapToEntityDto(Warehouse entity)
    {
        var dto = base.MapToEntityDto(entity);
        dto.Media = _mediaRepository.GetAll()
            .Where(m => m.MediaType == DMS.Media.MediaType.Warehouse && m.ModelId == entity.Id)
            .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
            .ToList();
        return dto;
    }

    protected override IQueryable<Warehouse> CreateFilteredQuery(PagedWarehouseResultRequestDto input)
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

    public async Task<ApiResponse<WarehouseWithProductsDto>> GetWarehouseWithProductsAsync(int id)
    {
        var warehouse = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<WarehouseWithProductsDto>(warehouse);

        var products = await _warehouseProductRepository.GetAll()
            .Where(wp => wp.WarehouseId == id)
            .ToListAsync();

        dto.WarehouseProducts = ObjectMapper.Map<List<WarehouseProductDto>>(products);
        return Ok(dto, L("SuccessfullyRetrieved"));
    }

    public async Task<ApiResponse<object>> AssignProductsAsync(WarehouseProductCreateDto input)
    {
        if (input.ProductWarehouses == null || input.ProductWarehouses.Count == 0)
            return Ok<object>(null, L("SavedSuccessfully"));

        var existing = await _warehouseProductRepository.GetAll()
            .Where(wp => wp.WarehouseId == input.WarehouseId)
            .ToListAsync();

        foreach (var item in existing)
            await _warehouseProductRepository.DeleteAsync(item);

        foreach (var item in input.ProductWarehouses)
        {
            await _warehouseProductRepository.InsertAsync(new WarehouseProduct
            {
                WarehouseId = input.WarehouseId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                WeightPerKG = item.WeightPerKG,
                TenantId = AbpSession.TenantId ?? 0
            });
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("SavedSuccessfully"));
    }
}
