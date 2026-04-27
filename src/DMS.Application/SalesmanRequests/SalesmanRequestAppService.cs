using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.SalesmanRequests.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.SalesmanRequests;

public class SalesmanRequestAppService : DMSAppServiceBase, ISalesmanRequestAppService
{
    private readonly IRepository<SalesmanRequest, int> _requestRepository;
    private readonly IRepository<SalesmanRequestItem, int> _itemRepository;

    public SalesmanRequestAppService(
        IRepository<SalesmanRequest, int> requestRepository,
        IRepository<SalesmanRequestItem, int> itemRepository)
    {
        _requestRepository = requestRepository;
        _itemRepository = itemRepository;
    }

    public async Task<ApiResponse<SalesmanRequestDto>> GetAsync(EntityDto<int> input)
    {
        var request = await _requestRepository.GetAsync(input.Id);
        var dto = MapToDto(request);
        var items = await _itemRepository.GetAll()
            .Where(x => x.RequestId == input.Id)
            .ToListAsync();
        dto.Items = items.Select(i => new SalesmanRequestItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<SalesmanRequestDto>>> GetAllAsync(PagedSalesmanRequestResultRequestDto input)
    {
        var query = _requestRepository.GetAll()
            .WhereIf(input.SalesmanId.HasValue, x => x.SalesmanId == input.SalesmanId.Value)
            .WhereIf(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value)
            .WhereIf(input.DateFrom.HasValue, x => x.RequestDate >= input.DateFrom.Value)
            .WhereIf(input.DateTo.HasValue, x => x.RequestDate <= input.DateTo.Value);

        var totalCount = await query.CountAsync();
        var requests = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
        var dtos = requests.Select(MapToDto).ToList();
        return Ok(new PagedResultDto<SalesmanRequestDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<SalesmanRequestDto>> CreateAsync(CreateSalesmanRequestDto input)
    {
        var request = new SalesmanRequest
        {
            SalesmanId = input.SalesmanId,
            WarehouseId = input.WarehouseId,
            RequestDate = input.RequestDate,
            Notes = input.Notes,
            Status = 0
        };
        await _requestRepository.InsertAsync(request);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new SalesmanRequestItem
            {
                RequestId = request.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = request.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapToDto(request);
        dto.Items = input.Items.Select(i => new SalesmanRequestItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<SalesmanRequestDto>> UpdateAsync(UpdateSalesmanRequestDto input)
    {
        var request = await _requestRepository.GetAsync(input.Id);
        request.SalesmanId = input.SalesmanId;
        request.WarehouseId = input.WarehouseId;
        request.RequestDate = input.RequestDate;
        request.Notes = input.Notes;

        var oldItems = await _itemRepository.GetAll().Where(x => x.RequestId == input.Id).ToListAsync();
        foreach (var old in oldItems)
            await _itemRepository.DeleteAsync(old);

        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new SalesmanRequestItem
            {
                RequestId = request.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = request.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapToDto(request);
        dto.Items = input.Items.Select(i => new SalesmanRequestItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var items = await _itemRepository.GetAll().Where(x => x.RequestId == input.Id).ToListAsync();
        foreach (var item in items)
            await _itemRepository.DeleteAsync(item);

        await _requestRepository.DeleteAsync(input.Id);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    public async Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input)
    {
        var request = await _requestRepository.GetAsync(input.Id);
        request.Status = 1;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> RejectAsync(RejectSalesmanRequestDto input)
    {
        var request = await _requestRepository.GetAsync(input.Id);
        request.Status = 2;
        request.RejectionReason = input.Reason;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    private static SalesmanRequestDto MapToDto(SalesmanRequest r) => new()
    {
        Id = r.Id,
        SalesmanId = r.SalesmanId,
        WarehouseId = r.WarehouseId,
        RequestDate = r.RequestDate,
        Notes = r.Notes,
        Status = r.Status,
        RejectionReason = r.RejectionReason
    };
}
