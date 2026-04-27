using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class SalesmanToWarehouseTransferAppService : DMSAppServiceBase, ISalesmanToWarehouseTransferAppService
{
    private readonly IRepository<SalesmanToWarehouseTransfer, int> _transferRepository;
    private readonly IRepository<SalesmanToWarehouseTransferItem, int> _itemRepository;

    public SalesmanToWarehouseTransferAppService(
        IRepository<SalesmanToWarehouseTransfer, int> transferRepository,
        IRepository<SalesmanToWarehouseTransferItem, int> itemRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
    }

    public async Task<ApiResponse<SalesmanToWarehouseTransferDto>> GetAsync(EntityDto<int> input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        var dto = MapToDto(transfer);
        var items = await _itemRepository.GetAll()
            .Where(x => x.TransferId == input.Id)
            .ToListAsync();
        dto.Items = items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<SalesmanToWarehouseTransferDto>>> GetAllAsync(PagedSalesmanToWarehouseTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.SalesmanId.HasValue, x => x.SalesmanId == input.SalesmanId.Value)
            .WhereIf(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
        var dtos = transfers.Select(MapToDto).ToList();
        return Ok(new PagedResultDto<SalesmanToWarehouseTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<SalesmanToWarehouseTransferDto>> CreateAsync(CreateSalesmanToWarehouseTransferDto input)
    {
        var transfer = new SalesmanToWarehouseTransfer
        {
            SalesmanId = input.SalesmanId,
            WarehouseId = input.WarehouseId,
            TransferDate = input.TransferDate,
            Notes = input.Notes,
            Status = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new SalesmanToWarehouseTransferItem
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

    public async Task<ApiResponse<SalesmanToWarehouseTransferDto>> UpdateAsync(UpdateSalesmanToWarehouseTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.SalesmanId = input.SalesmanId;
        transfer.WarehouseId = input.WarehouseId;
        transfer.TransferDate = input.TransferDate;
        transfer.Notes = input.Notes;
        transfer.Status = input.Status;

        var oldItems = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var old in oldItems)
            await _itemRepository.DeleteAsync(old);

        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            await _itemRepository.InsertAsync(new SalesmanToWarehouseTransferItem
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

    private static SalesmanToWarehouseTransferDto MapToDto(SalesmanToWarehouseTransfer t) => new()
    {
        Id = t.Id,
        SalesmanId = t.SalesmanId,
        WarehouseId = t.WarehouseId,
        TransferDate = t.TransferDate,
        Notes = t.Notes,
        Status = t.Status,
        RejectionReason = t.RejectionReason
    };
}
