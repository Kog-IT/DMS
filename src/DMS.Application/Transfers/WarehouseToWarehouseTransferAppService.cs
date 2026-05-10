using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Products;
using DMS.Transfers.Dto;
using DMS.Warehouses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class WarehouseToWarehouseTransferAppService : DMSAppServiceBase, IWarehouseToWarehouseTransferAppService
{
    private readonly IRepository<WarehouseToWarehouseTransfer, int> _transferRepository;
    private readonly IRepository<WarehouseToWarehouseTransferItem, int> _itemRepository;
    private readonly IRepository<Warehouse, int> _warehouseRepository;
    private readonly IRepository<Product, int> _productRepository;

    public WarehouseToWarehouseTransferAppService(
        IRepository<WarehouseToWarehouseTransfer, int> transferRepository,
        IRepository<WarehouseToWarehouseTransferItem, int> itemRepository,
        IRepository<Warehouse, int> warehouseRepository,
        IRepository<Product, int> productRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> GetAsync(EntityDto<int> input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        var dto = await EnrichAsync(transfer);
        var items = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        var pIds = items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetAll().Where(p => pIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name }).ToDictionaryAsync(p => p.Id, p => p.Name);
        dto.Items = items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity, ProductName = products.GetValueOrDefault(i.ProductId, "") }).ToList();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<PagedResultDto<WarehouseToWarehouseTransferDto>>> GetAllAsync(PagedWarehouseToWarehouseTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.FromWarehouseId.HasValue, x => x.FromWarehouseId == input.FromWarehouseId.Value)
            .WhereIf(input.ToWarehouseId.HasValue,   x => x.ToWarehouseId   == input.ToWarehouseId.Value)
            .WhereIf(input.Status.HasValue,           x => x.Status          == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers  = await query.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

        var wIds = transfers.SelectMany(t => new[] { t.FromWarehouseId, t.ToWarehouseId }).Distinct().ToList();
        var warehouses = await _warehouseRepository.GetAll()
            .Where(w => wIds.Contains(w.Id))
            .Select(w => new { w.Id, w.Name })
            .ToDictionaryAsync(w => w.Id, w => w.Name);

        var tIds = transfers.Select(t => t.Id).ToList();
        var itemCounts = await _itemRepository.GetAll()
            .Where(i => tIds.Contains(i.TransferId))
            .GroupBy(i => i.TransferId)
            .Select(g => new { TransferId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.TransferId, g => g.Count);

        var dtos = transfers.Select(t => new WarehouseToWarehouseTransferDto
        {
            Id                = t.Id,
            FromWarehouseId   = t.FromWarehouseId,
            ToWarehouseId     = t.ToWarehouseId,
            TransferDate      = t.TransferDate,
            Notes             = t.Notes,
            Status            = t.Status,
            TransactionNumber = $"W2W-{t.Id:D5}",
            FromWarehouseName = warehouses.GetValueOrDefault(t.FromWarehouseId, ""),
            ToWarehouseName   = warehouses.GetValueOrDefault(t.ToWarehouseId,   ""),
            ProductsQuantity  = itemCounts.GetValueOrDefault(t.Id, 0),
            CreatedDate       = t.CreationTime,
        }).ToList();

        return Ok(new PagedResultDto<WarehouseToWarehouseTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> CreateAsync(CreateWarehouseToWarehouseTransferDto input)
    {
        var transfer = new WarehouseToWarehouseTransfer
        {
            FromWarehouseId = input.FromWarehouseId,
            ToWarehouseId   = input.ToWarehouseId,
            TransferDate    = input.TransferDate,
            Notes           = input.Notes,
            Status          = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new WarehouseToWarehouseTransferItem
            { TransferId = transfer.Id, ProductId = item.ProductId, Quantity = item.Quantity, TenantId = transfer.TenantId });
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = await EnrichAsync(transfer);
        dto.Items = input.Items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToWarehouseTransferDto>> UpdateAsync(UpdateWarehouseToWarehouseTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.FromWarehouseId = input.FromWarehouseId;
        transfer.ToWarehouseId   = input.ToWarehouseId;
        transfer.TransferDate    = input.TransferDate;
        transfer.Notes           = input.Notes;
        transfer.Status          = input.Status;

        var old = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var o in old) await _itemRepository.DeleteAsync(o);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new WarehouseToWarehouseTransferItem
            { TransferId = transfer.Id, ProductId = item.ProductId, Quantity = item.Quantity, TenantId = transfer.TenantId });
        await CurrentUnitOfWork.SaveChangesAsync();

        var dto = await EnrichAsync(transfer);
        dto.Items = input.Items.Select(i => new TransferItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList();
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var items = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var item in items) await _itemRepository.DeleteAsync(item);
        await _transferRepository.DeleteAsync(input.Id);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    private async Task<WarehouseToWarehouseTransferDto> EnrichAsync(WarehouseToWarehouseTransfer t)
    {
        var whs = await _warehouseRepository.GetAll()
            .Where(w => w.Id == t.FromWarehouseId || w.Id == t.ToWarehouseId)
            .Select(w => new { w.Id, w.Name }).ToListAsync();
        return new WarehouseToWarehouseTransferDto
        {
            Id = t.Id, FromWarehouseId = t.FromWarehouseId, ToWarehouseId = t.ToWarehouseId,
            TransferDate = t.TransferDate, Notes = t.Notes, Status = t.Status,
            TransactionNumber = $"W2W-{t.Id:D5}",
            FromWarehouseName = whs.FirstOrDefault(w => w.Id == t.FromWarehouseId)?.Name ?? "",
            ToWarehouseName   = whs.FirstOrDefault(w => w.Id == t.ToWarehouseId)?.Name   ?? "",
            CreatedDate = t.CreationTime,
        };
    }
}
