using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Common.Dto;
using DMS.CustomerGroups.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.CustomerGroups;

public class CustomerGroupAppService : DmsCrudAppService<
    CustomerGroup, CustomerGroupDto, int,
    PagedCustomerGroupResultRequestDto,
    CreateCustomerGroupDto, UpdateCustomerGroupDto>, ICustomerGroupAppService
{
    public CustomerGroupAppService(IRepository<CustomerGroup, int> repository) : base(repository) { }

    protected override IQueryable<CustomerGroup> CreateFilteredQuery(PagedCustomerGroupResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Keyword))
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

    public async Task<ApiResponse<object>> UpdateTaxExemptedAsync(UpdateCustomerGroupTaxExemptedDto input)
    {
        var entity = await Repository.GetAsync(input.Id);
        entity.IsTaxExempted = input.IsTaxExempted;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }
}
