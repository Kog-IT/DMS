using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class WarehouseToWarehouseTransferAppService : DMSAppServiceBase, IWarehouseToWarehouseTransferAppService
{
    private readonly IRepository<WarehouseToWarehouseTransfer, int> _transferRepository;
    private readonly IRepository<WarehouseToWarehouseTransferItem, int> _itemRepository;

    public WarehouseToWarehouseTransferAppService(
        IRepository<WarehouseToWarehouseTransfer, int> transferRepository,
        IRepository<WarehouseToWarehouseTransferItem, int> itemRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> GetAsync(EntityDto<int> input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        var dto = MapTransferToDto(transfer);
        var items = await _itemRepository.GetAll()
            .Where(x => x.TransferId == input.Id)
            .ToListAsync();
        dto.Items = items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<WarehouseToWarehouseTransferDto>>> GetAllAsync(PagedWarehouseToWarehouseTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.FromWarehouseId.HasValue, x => x.FromWarehouseId == input.FromWarehouseId.Value)
            .WhereIf(input.ToWarehouseId.HasValue, x => x.ToWarehouseId == input.ToWarehouseId.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
        var dtos = transfers.Select(MapTransferToDto).ToList();
        return Ok(new PagedResultDto<WarehouseToWarehouseTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> CreateAsync(CreateWarehouseToWarehouseTransferDto input)
    {
        var transfer = new WarehouseToWarehouseTransfer
        {
            FromWarehouseId = input.FromWarehouseId,
            ToWarehouseId = input.ToWarehouseId,
            TransferDate = input.TransferDate,
            Notes = input.Notes,
            Status = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new WarehouseToWarehouseTransferItem
            {
                TransferId = transfer.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = transfer.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapTransferToDto(transfer);
        dto.Items = input.Items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> UpdateAsync(UpdateWarehouseToWarehouseTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.FromWarehouseId = input.FromWarehouseId;
        transfer.ToWarehouseId = input.ToWarehouseId;
        transfer.TransferDate = input.TransferDate;
        transfer.Notes = input.Notes;
        transfer.Status = input.Status;

        var oldItems = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var old in oldItems)
            await _itemRepository.DeleteAsync(old);

        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new WarehouseToWarehouseTransferItem
            {
                TransferId = transfer.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = transfer.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapTransferToDto(transfer);
        dto.Items = input.Items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var items = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var item in items)
            await _itemRepository.DeleteAsync(item);

        await _transferRepository.DeleteAsync(input.Id);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    private static WarehouseToWarehouseTransferDto MapTransferToDto(WarehouseToWarehouseTransfer t) => new()
    {
        Id = t.Id,
        FromWarehouseId = t.FromWarehouseId,
        ToWarehouseId = t.ToWarehouseId,
        TransferDate = t.TransferDate,
        Notes = t.Notes,
        Status = t.Status
    };
}
