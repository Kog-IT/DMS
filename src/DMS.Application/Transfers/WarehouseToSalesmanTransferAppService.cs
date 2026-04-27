using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class WarehouseToSalesmanTransferAppService : DMSAppServiceBase, IWarehouseToSalesmanTransferAppService
{
    private readonly IRepository<WarehouseToSalesmanTransfer, int> _transferRepository;
    private readonly IRepository<WarehouseToSalesmanTransferItem, int> _itemRepository;

    public WarehouseToSalesmanTransferAppService(
        IRepository<WarehouseToSalesmanTransfer, int> transferRepository,
        IRepository<WarehouseToSalesmanTransferItem, int> itemRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> GetAsync(EntityDto<int> input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        var dto = MapToDto(transfer);
        var items = await _itemRepository.GetAll()
            .Where(x => x.TransferId == input.Id)
            .ToListAsync();
        dto.Items = items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<WarehouseToSalesmanTransferDto>>> GetAllAsync(PagedWarehouseToSalesmanTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId.Value)
            .WhereIf(input.SalesmanId.HasValue, x => x.SalesmanId == input.SalesmanId.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
        var dtos = transfers.Select(MapToDto).ToList();
        return Ok(new PagedResultDto<WarehouseToSalesmanTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> CreateAsync(CreateWarehouseToSalesmanTransferDto input)
    {
        var transfer = new WarehouseToSalesmanTransfer
        {
            WarehouseId = input.WarehouseId,
            SalesmanId = input.SalesmanId,
            TransferDate = input.TransferDate,
            Notes = input.Notes,
            Status = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new WarehouseToSalesmanTransferItem
            {
                TransferId = transfer.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = transfer.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapToDto(transfer);
        dto.Items = input.Items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> UpdateAsync(UpdateWarehouseToSalesmanTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.WarehouseId = input.WarehouseId;
        transfer.SalesmanId = input.SalesmanId;
        transfer.TransferDate = input.TransferDate;
        transfer.Notes = input.Notes;
        transfer.Status = input.Status;

        var oldItems = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var old in oldItems)
            await _itemRepository.DeleteAsync(old);

        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new WarehouseToSalesmanTransferItem
            {
                TransferId = transfer.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TenantId = transfer.TenantId
            });
        }
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = MapToDto(transfer);
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

    public async Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.Status = 1;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> RejectAsync(RejectTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.Status = 2;
        transfer.RejectionReason = input.Reason;
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    private static WarehouseToSalesmanTransferDto MapToDto(WarehouseToSalesmanTransfer t) => new()
    {
        Id = t.Id,
        WarehouseId = t.WarehouseId,
        SalesmanId = t.SalesmanId,
        TransferDate = t.TransferDate,
        Notes = t.Notes,
        Status = t.Status,
        RejectionReason = t.RejectionReason
    };
}
