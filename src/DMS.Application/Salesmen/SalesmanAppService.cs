using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Salesmen.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Salesmen;

public class SalesmanAppService : DmsCrudAppService<
    Salesman, SalesmanDto, int,
    PagedSalesmanResultRequestDto,
    CreateSalesmanDto, UpdateSalesmanDto>, ISalesmanAppService
{
    public SalesmanAppService(IRepository<Salesman, int> repository) : base(repository) { }

    protected override IQueryable<Salesman> CreateFilteredQuery(PagedSalesmanResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Keyword)
                  || x.Code.Contains(input.Keyword)
                  || x.Mobile.Contains(input.Keyword))
            .WhereIf(input.GovernorateId.HasValue, x => x.GovernorateId == input.GovernorateId.Value)
            .WhereIf(input.CityId.HasValue, x => x.CityId == input.CityId.Value)
            .WhereIf(input.WarehouseId.HasValue, x => x.AssignedWarehouseId == input.WarehouseId.Value)
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

    public async Task<ApiResponse<List<SalesmanSelectItemDto>>> GetSelectListAsync()
    {
        var list = await Repository.GetAll()
            .Where(s => s.IsActive)
            .Select(s => new SalesmanSelectItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code
            })
            .ToListAsync();
        return Ok(list, L("RetrievedSuccessfully"));
    }
}
