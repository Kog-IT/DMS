using Abp.Application.Services.Dto;
using DMS.Transfers;
using DMS.Transfers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Transfers;

public class TransferAppService_Tests : DMSTestBase
{
    private readonly IWarehouseToWarehouseTransferAppService _w2wService;
    private readonly IWarehouseToSalesmanTransferAppService _w2sService;
    private readonly ISalesmanToWarehouseTransferAppService _s2wService;
    private readonly ISalesmanToSalesmanTransferAppService _s2sService;

    public TransferAppService_Tests()
    {
        _w2wService = Resolve<IWarehouseToWarehouseTransferAppService>();
        _w2sService = Resolve<IWarehouseToSalesmanTransferAppService>();
        _s2wService = Resolve<ISalesmanToWarehouseTransferAppService>();
        _s2sService = Resolve<ISalesmanToSalesmanTransferAppService>();
    }

    [Fact]
    public async Task CreateWarehouseToWarehouseTransfer_ShouldPersist()
    {
        var result = await _w2wService.CreateAsync(new CreateWarehouseToWarehouseTransferDto
        {
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            TransferDate = new DateTime(2026, 1, 10),
            Notes = "W2W test",
            Items = new List<CreateTransferItemDto>
            {
                new CreateTransferItemDto { ProductId = 1, Quantity = 5 }
            }
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.FromWarehouseId.ShouldBe(1);

        await UsingDbContextAsync(async context =>
        {
            var transfer = await context.Set<WarehouseToWarehouseTransfer>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            transfer.ShouldNotBeNull();

            var itemCount = await context.Set<WarehouseToWarehouseTransferItem>()
                .CountAsync(x => x.TransferId == result.Data.Id);
            itemCount.ShouldBe(1);
        });
    }

    [Fact]
    public async Task CreateWarehouseToSalesmanTransfer_ShouldPersistAndApprove()
    {
        var result = await _w2sService.CreateAsync(new CreateWarehouseToSalesmanTransferDto
        {
            WarehouseId = 1,
            SalesmanId = 1,
            TransferDate = new DateTime(2026, 1, 10),
            Notes = "W2S test",
            Items = new List<CreateTransferItemDto>
            {
                new CreateTransferItemDto { ProductId = 1, Quantity = 3 }
            }
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        await _w2sService.ApproveAsync(new EntityDto<int> { Id = result.Data.Id });

        await UsingDbContextAsync(async context =>
        {
            var transfer = await context.Set<WarehouseToSalesmanTransfer>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            transfer.ShouldNotBeNull();
            transfer.Status.ShouldBe(1);
        });
    }

    [Fact]
    public async Task CreateSalesmanToWarehouseTransfer_ShouldPersistAndReject()
    {
        var result = await _s2wService.CreateAsync(new CreateSalesmanToWarehouseTransferDto
        {
            SalesmanId = 1,
            WarehouseId = 1,
            TransferDate = new DateTime(2026, 1, 10),
            Notes = "S2W test",
            Items = new List<CreateTransferItemDto>
            {
                new CreateTransferItemDto { ProductId = 1, Quantity = 2 }
            }
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        await _s2wService.RejectAsync(new RejectTransferDto { Id = result.Data.Id, Reason = "Out of stock" });

        await UsingDbContextAsync(async context =>
        {
            var transfer = await context.Set<SalesmanToWarehouseTransfer>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            transfer.ShouldNotBeNull();
            transfer.Status.ShouldBe(2);
            transfer.RejectionReason.ShouldBe("Out of stock");
        });
    }

    [Fact]
    public async Task CreateSalesmanToSalesmanTransfer_ShouldPersist()
    {
        var result = await _s2sService.CreateAsync(new CreateSalesmanToSalesmanTransferDto
        {
            FromSalesmanId = 1,
            ToSalesmanId = 2,
            TransferDate = new DateTime(2026, 1, 10),
            Notes = "S2S test",
            Items = new List<CreateTransferItemDto>
            {
                new CreateTransferItemDto { ProductId = 1, Quantity = 4 }
            }
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        await UsingDbContextAsync(async context =>
        {
            var transfer = await context.Set<SalesmanToSalesmanTransfer>()
                .FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            transfer.ShouldNotBeNull();
            transfer.FromSalesmanId.ShouldBe(1);
        });
    }
}
