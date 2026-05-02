using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common.Dto;
using DMS.Products;
using DMS.Salesmen;
using DMS.Transfers.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Transfers;

public class SalesmanToSalesmanTransferAppService : DMSAppServiceBase, ISalesmanToSalesmanTransferAppService
{
    private readonly IRepository<SalesmanToSalesmanTransfer, int> _transferRepository;
    private readonly IRepository<SalesmanToSalesmanTransferItem, int> _itemRepository;
    private readonly IRepository<Salesman, int> _salesmanRepository;
    private readonly IRepository<Product, int> _productRepository;

    public SalesmanToSalesmanTransferAppService(
        IRepository<SalesmanToSalesmanTransfer, int> transferRepository,
        IRepository<SalesmanToSalesmanTransferItem, int> itemRepository,
        IRepository<Salesman, int> salesmanRepository,
        IRepository<Product, int> productRepository)
    {
        _transferRepository = transferRepository;
        _itemRepository = itemRepository;
        _salesmanRepository = salesmanRepository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<SalesmanToSalesmanTransferDto>> GetAsync(EntityDto<int> input)
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

    public async Task<ApiResponse<PagedResultDto<SalesmanToSalesmanTransferDto>>> GetAllAsync(PagedSalesmanToSalesmanTransferRequestDto input)
    {
        var query = _transferRepository.GetAll()
            .WhereIf(input.FromSalesmanId.HasValue, x => x.FromSalesmanId == input.FromSalesmanId.Value)
            .WhereIf(input.ToSalesmanId.HasValue,   x => x.ToSalesmanId   == input.ToSalesmanId.Value)
            .WhereIf(input.Status.HasValue,         x => x.Status          == input.Status.Value);

        var totalCount = await query.CountAsync();
        var transfers  = await query.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

        var sIds = transfers.SelectMany(t => new[] { t.FromSalesmanId, t.ToSalesmanId }).Distinct().ToList();
        var salesmen = await _salesmanRepository.GetAll().Where(s => sIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Name }).ToDictionaryAsync(s => s.Id, s => s.Name);

        var tIds = transfers.Select(t => t.Id).ToList();
        var itemCounts = await _itemRepository.GetAll().Where(i => tIds.Contains(i.TransferId))
            .GroupBy(i => i.TransferId)
            .Select(g => new { TransferId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.TransferId, g => g.Count);

        var dtos = transfers.Select(t => new SalesmanToSalesmanTransferDto
        {
            Id                = t.Id,
            FromSalesmanId    = t.FromSalesmanId,
            ToSalesmanId      = t.ToSalesmanId,
            TransferDate      = t.TransferDate,
            Notes             = t.Notes,
            Status            = t.Status,
            RejectionReason   = t.RejectionReason,
            TransactionNumber = $"S2S-{t.Id:D5}",
            FromSalesManName  = salesmen.GetValueOrDefault(t.FromSalesmanId, ""),
            ToSalesMan        = salesmen.GetValueOrDefault(t.ToSalesmanId,   ""),
            ProductsQuantity  = itemCounts.GetValueOrDefault(t.Id, 0),
            CreatedDate       = t.CreationTime,
        }).ToList();

        return Ok(new PagedResultDto<SalesmanToSalesmanTransferDto>(totalCount, dtos), L("RetrievedSuccessfully"));
    }

    public async Task<ApiResponse<SalesmanToSalesmanTransferDto>> CreateAsync(CreateSalesmanToSalesmanTransferDto input)
    {
        var transfer = new SalesmanToSalesmanTransfer
        {
            FromSalesmanId = input.FromSalesmanId,
            ToSalesmanId   = input.ToSalesmanId,
            TransferDate   = input.TransferDate,
            Notes          = input.Notes,
            Status         = 0
        };
        await _transferRepository.InsertAsync(transfer);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new SalesmanToSalesmanTransferItem
            { TransferId = transfer.Id, ProductId = item.ProductId, Quantity = item.Quantity, TenantId = transfer.TenantId });
        await CurrentUnitOfWork.SaveChangesAsync();

        return Ok(await EnrichAsync(transfer), L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<SalesmanToSalesmanTransferDto>> UpdateAsync(UpdateSalesmanToSalesmanTransferDto input)
    {
        var transfer = await _transferRepository.GetAsync(input.Id);
        transfer.FromSalesmanId = input.FromSalesmanId;
        transfer.ToSalesmanId   = input.ToSalesmanId;
        transfer.TransferDate   = input.TransferDate;
        transfer.Notes          = input.Notes;

        var old = await _itemRepository.GetAll().Where(x => x.TransferId == input.Id).ToListAsync();
        foreach (var o in old) await _itemRepository.DeleteAsync(o);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var item in input.Items)
            await _itemRepository.InsertAsync(new SalesmanToSalesmanTransferItem
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

    private async Task<SalesmanToSalesmanTransferDto> EnrichAsync(SalesmanToSalesmanTransfer t)
    {
        var sNames = await _salesmanRepository.GetAll()
            .Where(s => s.Id == t.FromSalesmanId || s.Id == t.ToSalesmanId)
            .Select(s => new { s.Id, s.Name }).ToListAsync();
        return new SalesmanToSalesmanTransferDto
        {
            Id = t.Id, FromSalesmanId = t.FromSalesmanId, ToSalesmanId = t.ToSalesmanId,
            TransferDate = t.TransferDate, Notes = t.Notes, Status = t.Status, RejectionReason = t.RejectionReason,
            TransactionNumber = $"S2S-{t.Id:D5}",
            FromSalesManName = sNames.FirstOrDefault(s => s.Id == t.FromSalesmanId)?.Name ?? "",
            ToSalesMan       = sNames.FirstOrDefault(s => s.Id == t.ToSalesmanId)?.Name   ?? "",
            CreatedDate = t.CreationTime,
        };
    }
}
