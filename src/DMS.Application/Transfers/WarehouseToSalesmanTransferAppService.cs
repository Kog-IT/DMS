using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Products;
using DMS.Salesmen;
using DMS.Transfers.Dto;
using DMS.Warehouses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class WarehouseToSalesmanTransferAppService : DMSAppServiceBase, IWarehouseToSalesmanTransferAppService
{
    private readonly IRepository<WarehouseToSalesmanTransfer, int> _transferRepository;
    private readonly IRepository<WarehouseToSalesmanTransferItem, int> _itemRepository;
    private readonly IRepository<Warehouse, int> _warehouseRepository;
    private readonly IRepository<Salesman, int> _salesmanRepository;
    private readonly IRepository<Product, int> _productRepository;

    public WarehouseToSalesmanTransferAppService(
        IRepository<WarehouseToSalesmanTransfer, int> transferRepository,
        IRepository<WarehouseToSalesmanTransferItem, int> itemRepository,
        IRepository<Warehouse, int> warehouseRepository,
        IRepository<Salesman, int> salesmanRepository,
        IRepository<Product, int> productRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _salesmanRepository = salesmanRepository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> GetAsync(EntityDto<int> input)
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

    public async Task<ApiResponse<PagedResultDto<WarehouseToSalesmanTransferDto>>> GetAllAsync(PagedWarehouseToSalesmanTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId.Value)
            .WhereIf(input.SalesmanId.HasValue,  x => x.SalesmanId  == input.SalesmanId.Value)
            .WhereIf(input.Status.HasValue,      x => x.Status       == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers  = await query.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

        var wIds = transfers.Select(t => t.WarehouseId).Distinct().ToList();
        var sIds = transfers.Select(t => t.SalesmanId).Distinct().ToList();
        var warehouses = await _warehouseRepository.GetAll().Where(w => wIds.Contains(w.Id))
            .Select(w => new { w.Id, w.Name }).ToDictionaryAsync(w => w.Id, w => w.Name);
        var salesmen = await _salesmanRepository.GetAll().Where(s => sIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Name }).ToDictionaryAsync(s => s.Id, s => s.Name);

        var tIds = transfers.Select(t => t.Id).ToList();
        var itemCounts = await _itemRepository.GetAll().Where(i => tIds.Contains(i.TransferId))
            .GroupBy(i => i.TransferId)
            .Select(g => new { TransferId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.TransferId, g => g.Count);

        var dtos = transfers.Select(t => new WarehouseToSalesmanTransferDto
        {
            Id                = t.Id,
            WarehouseId       = t.WarehouseId,
            SalesmanId        = t.SalesmanId,
            TransferDate      = t.TransferDate,
            Notes             = t.Notes,
            Status            = t.Status,
            RejectionReason   = t.RejectionReason,
            TransactionNumber = $"W2S-{t.Id:D5}",
            WarehouseName     = warehouses.GetValueOrDefault(t.WarehouseId, ""),
            SalesManName      = salesmen.GetValueOrDefault(t.SalesmanId,    ""),
            ProductNumber     = itemCounts.GetValueOrDefault(t.Id, 0),
            CreatedDate       = t.CreationTime,
        }).ToList();

        return Ok(new PagedResultDto<WarehouseToSalesmanTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> CreateAsync(CreateWarehouseToSalesmanTransferDto input)
    {
        var transfer = new WarehouseToSalesmanTransfer
        {
            WarehouseId  = input.WarehouseId,
            SalesmanId   = input.SalesmanId,
            TransferDate = input.TransferDate,
            Notes        = input.Notes,
            Status       = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new WarehouseToSalesmanTransferItem
            { TransferId = transfer.Id, ProductId = item.ProductId, Quantity = item.Quantity, TenantId = transfer.TenantId });
        await CurrentUnitOfWork.SaveChangesAsync();

        return Ok(await EnrichAsync(transfer), L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<WarehouseToSalesmanTransferDto>> UpdateAsync(UpdateWarehouseToSalesmanTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.WarehouseId  = input.WarehouseId;
        transfer.SalesmanId   = input.SalesmanId;
        transfer.TransferDate = input.TransferDate;
        transfer.Notes        = input.Notes;

        var old = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var o in old) await _itemRepository.DeleteAsync(o);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new WarehouseToSalesmanTransferItem
            { TransferId = transfer.Id, ProductId = item.ProductId, Quantity = item.Quantity, TenantId = transfer.TenantId });
        await CurrentUnitOfWork.SaveChangesAsync();

        return Ok(await EnrichAsync(transfer), L("UpdatedSuccessfully"));
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

    public async Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input)
    {
        var items = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var item in items) await _itemRepository.DeleteAsync(item);
        await _transferRepository.DeleteAsync(input.Id);
        return Ok<object>(null, L("DeletedSuccessfully"));
    }

    private async Task<WarehouseToSalesmanTransferDto> EnrichAsync(WarehouseToSalesmanTransfer t)
    {
        var wName = (await _warehouseRepository.GetAll().Where(w => w.Id == t.WarehouseId).Select(w => w.Name).FirstOrDefaultAsync()) ?? "";
        var sName = (await _salesmanRepository.GetAll().Where(s => s.Id == t.SalesmanId).Select(s => s.Name).FirstOrDefaultAsync()) ?? "";
        return new WarehouseToSalesmanTransferDto
        {
            Id = t.Id, WarehouseId = t.WarehouseId, SalesmanId = t.SalesmanId,
            TransferDate = t.TransferDate, Notes = t.Notes, Status = t.Status, RejectionReason = t.RejectionReason,
            TransactionNumber = $"W2S-{t.Id:D5}", WarehouseName = wName, SalesManName = sName, CreatedDate = t.CreationTime,
        };
    }
}
