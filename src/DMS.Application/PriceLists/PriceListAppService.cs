using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.PriceLists.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.PriceLists;

[AbpAuthorize(PermissionNames.Pages_PriceLists)]
public class PriceListAppService : DmsCrudAppService<
    PriceList,
    PriceListDto,
    int,
    PagedPriceListRequestDto,
    CreatePriceListDto,
    UpdatePriceListDto>, IPriceListAppService
{
    private readonly IRepository<PriceListItem, int> _itemRepository;
    private readonly IRepository<PriceListAssignment, int> _assignmentRepository;

    public PriceListAppService(
        IRepository<PriceList, int> repository,
        IRepository<PriceListItem, int> itemRepository,
        IRepository<PriceListAssignment, int> assignmentRepository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_PriceLists;
        GetAllPermissionName = PermissionNames.Pages_PriceLists;
        CreatePermissionName = PermissionNames.Pages_PriceLists_Create;
        UpdatePermissionName = PermissionNames.Pages_PriceLists_Edit;
        DeletePermissionName = PermissionNames.Pages_PriceLists_Delete;

        _itemRepository = itemRepository;
        _assignmentRepository = assignmentRepository;
    }

    protected override IQueryable<PriceList> CreateFilteredQuery(PagedPriceListRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), p => p.Name.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, p => p.IsActive == input.IsActive.Value)
            .WhereIf(input.ForClassification.HasValue, p => p.ForClassification == input.ForClassification.Value)
            .WhereIf(input.ActiveOn.HasValue,
                p => p.StartDate <= input.ActiveOn.Value &&
                     (p.EndDate == null || p.EndDate >= input.ActiveOn.Value));
    }

    public override async Task<ApiResponse<PriceListDto>> CreateAsync(CreatePriceListDto input)
    {
        if (input.EndDate.HasValue && input.EndDate.Value <= input.StartDate)
            throw new UserFriendlyException("EndDate must be after StartDate.");

        return await base.CreateAsync(input);
    }

    public override async Task<ApiResponse<PriceListDto>> UpdateAsync(UpdatePriceListDto input)
    {
        if (input.EndDate.HasValue && input.EndDate.Value <= input.StartDate)
            throw new UserFriendlyException("EndDate must be after StartDate.");

        return await base.UpdateAsync(input);
    }

    public override async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var hasAssignments = await _assignmentRepository.GetAll()
            .AnyAsync(a => a.PriceListId == input.Id);

        if (hasAssignments)
            throw new UserFriendlyException(
                "Cannot delete a price list that is assigned to customers. Remove all assignments first.");

        return await base.DeleteAsync(input);
    }

    [AbpAuthorize(PermissionNames.Pages_PriceLists)]
    public async Task<ApiResponse<List<PriceListItemDto>>> GetItemsAsync(int priceListId)
    {
        var items = await _itemRepository.GetAll()
            .Where(i => i.PriceListId == priceListId)
            .OrderBy(i => i.ProductId)
            .ThenBy(i => i.MinQuantity)
            .ToListAsync();

        var dto = ObjectMapper.Map<List<PriceListItemDto>>(items);
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_PriceLists_Edit)]
    public async Task<ApiResponse<object>> SetItemsAsync(SetPriceListItemsDto input)
    {
        var hasDuplicates = input.Items
            .GroupBy(i => new { i.ProductId, i.MinQuantity })
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
            throw new UserFriendlyException("Duplicate ProductId + MinQuantity combination in items.");

        var existing = await _itemRepository.GetAll()
            .Where(i => i.PriceListId == input.PriceListId)
            .ToListAsync();

        foreach (var item in existing)
            await _itemRepository.DeleteAsync(item);

        foreach (var itemInput in input.Items)
        {
            await _itemRepository.InsertAsync(new PriceListItem
            {
                PriceListId = input.PriceListId,
                ProductId = itemInput.ProductId,
                MinQuantity = itemInput.MinQuantity,
                Price = itemInput.Price
            });
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }
}
