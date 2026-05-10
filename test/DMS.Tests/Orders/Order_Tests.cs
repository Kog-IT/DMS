using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Runtime.Validation;
using Abp.UI;
using DMS.Categories;
using DMS.Customers;
using DMS.Orders;
using DMS.Orders.Dto;
using DMS.Products;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Orders;

public class Order_Tests : DMSTestBase
{
    private readonly IOrderAppService _orderService;

    public Order_Tests()
    {
        _orderService = Resolve<IOrderAppService>();
    }

    private async Task<int> SeedCustomerAsync(string code)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var c = new Customer { TenantId = 1, Code = code, Name = "Test Customer", IsActive = true };
            ctx.Set<Customer>().Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        });
        return id;
    }

    private async Task<int> SeedProductAsync(string name, decimal price)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var category = await ctx.Set<Category>().FirstOrDefaultAsync();
            if (category == null)
            {
                category = new Category { TenantId = 1, Name = "Default" };
                ctx.Set<Category>().Add(category);
                await ctx.SaveChangesAsync();
            }

            var p = new Product { TenantId = 1, Name = name, Price = price, CategoryId = category.Id };
            ctx.Set<Product>().Add(p);
            await ctx.SaveChangesAsync();
            id = p.Id;
        });
        return id;
    }

    [Fact]
    public async Task Create_Order_Succeeds_And_Computes_Totals()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C1");
        var productId = await SeedProductAsync("Widget", 100m);

        var result = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 2 } }
        });

        result.Data.Id.ShouldBeGreaterThan(0);
        result.Data.SubTotal.ShouldBe(200m);
        result.Data.Total.ShouldBe(200m);
        result.Data.Status.ShouldBe(OrderStatus.Draft);
    }

    [Fact]
    public async Task Create_Order_With_No_Lines_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C2");

        await Should.ThrowAsync<AbpValidationException>(async () =>
            await _orderService.CreateAsync(new CreateOrderDto
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Lines = new()
            })
        );
    }

    [Fact]
    public async Task Submit_Draft_Goes_To_Confirmed_When_No_Discount_Limit_Exceeded()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C3");
        var productId = await SeedProductAsync("Widget3", 50m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });

        // DiscountLimitSalesRep defaults to "0" meaning no limit → goes straight to Confirmed
        await _orderService.SubmitAsync(order.Data.Id);

        var updated = await _orderService.GetAsync(new EntityDto<int>(order.Data.Id));
        updated.Data.Status.ShouldBe(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Submit_Draft_Goes_To_PendingApproval_When_Discount_Exceeds_Limit()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C4");
        var productId = await SeedProductAsync("Widget4", 100m);

        var settingManager = Resolve<ISettingManager>();
        await settingManager.ChangeSettingForTenantAsync(1, "Orders.DiscountLimitSalesRep", "5");

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new()
            {
                new CreateOrderLineDto
                {
                    ProductId = productId,
                    Quantity = 1,
                    DiscountType = DiscountType.Percentage,
                    DiscountValue = 10 // 10 > 5 limit
                }
            }
        });

        await _orderService.SubmitAsync(order.Data.Id);

        var updated = await _orderService.GetAsync(new EntityDto<int>(order.Data.Id));
        updated.Data.Status.ShouldBe(OrderStatus.PendingApproval);
    }

    [Fact]
    public async Task Approve_Order_Transitions_To_Confirmed()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C5");
        var productId = await SeedProductAsync("Widget5", 100m);

        var settingManager = Resolve<ISettingManager>();
        await settingManager.ChangeSettingForTenantAsync(1, "Orders.DiscountLimitSalesRep", "5");

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new()
            {
                new CreateOrderLineDto
                {
                    ProductId = productId, Quantity = 1,
                    DiscountType = DiscountType.Percentage, DiscountValue = 10
                }
            }
        });
        await _orderService.SubmitAsync(order.Data.Id);

        await _orderService.ApproveAsync(order.Data.Id);

        var updated = await _orderService.GetAsync(new EntityDto<int>(order.Data.Id));
        updated.Data.Status.ShouldBe(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Reject_Order_Sets_RejectionReason_And_Cancels()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C6");
        var productId = await SeedProductAsync("Widget6", 100m);

        var settingManager = Resolve<ISettingManager>();
        await settingManager.ChangeSettingForTenantAsync(1, "Orders.DiscountLimitSalesRep", "5");

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new()
            {
                new CreateOrderLineDto
                {
                    ProductId = productId, Quantity = 1,
                    DiscountType = DiscountType.Percentage, DiscountValue = 10
                }
            }
        });
        await _orderService.SubmitAsync(order.Data.Id);

        await _orderService.RejectAsync(order.Data.Id, "Too high discount");

        var updated = await _orderService.GetAsync(new EntityDto<int>(order.Data.Id));
        updated.Data.Status.ShouldBe(OrderStatus.Cancelled);
        updated.Data.RejectionReason.ShouldBe("Too high discount");
    }

    [Fact]
    public async Task Edit_Confirmed_Order_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C7");
        var productId = await SeedProductAsync("Widget7", 50m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });
        await _orderService.SubmitAsync(order.Data.Id); // → Confirmed (no limit)

        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _orderService.UpdateAsync(new UpdateOrderDto
            {
                Id = order.Data.Id,
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 2 } }
            })
        );
    }

    [Fact]
    public async Task Cancel_Confirmed_Order_Succeeds()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C8");
        var productId = await SeedProductAsync("Widget8", 50m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });
        await _orderService.SubmitAsync(order.Data.Id); // → Confirmed

        await _orderService.CancelAsync(order.Data.Id);

        var updated = await _orderService.GetAsync(new EntityDto<int>(order.Data.Id));
        updated.Data.Status.ShouldBe(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Create_Line_With_Insufficient_Stock_Throws_When_BackOrder_Disabled()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C9");
        var productId = await SeedProductAsync("Widget9", 50m);

        var settingManager = Resolve<ISettingManager>();
        await settingManager.ChangeSettingForTenantAsync(1, "Orders.AllowOrdersWithoutStock", "false");

        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _orderService.CreateAsync(new CreateOrderDto
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
            })
        );
    }

    [Fact]
    public async Task Create_Line_Sets_IsBackOrder_False_When_BackOrder_Enabled()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("ORD_C10");
        var productId = await SeedProductAsync("Widget10", 50m);

        // AllowOrdersWithoutStock defaults to true
        var result = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });

        result.Data.Lines.ShouldNotBeEmpty();
        result.Data.Lines[0].IsBackOrder.ShouldBeFalse();
    }
}
