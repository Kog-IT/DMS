using Abp.Application.Services.Dto;
using DMS.Categories;
using DMS.Customers;
using DMS.Customers.Dto;
using DMS.Invoices;
using DMS.Orders;
using DMS.Orders.Dto;
using DMS.Products;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Customers;

public class CreditManagement_Tests : DMSTestBase
{
    private readonly ICustomerAppService _customerService;
    private readonly IOrderAppService _orderService;

    public CreditManagement_Tests()
    {
        _customerService = Resolve<ICustomerAppService>();
        _orderService = Resolve<IOrderAppService>();
    }

    private async Task<int> SeedCustomerAsync(string code, bool creditEnabled = false, decimal creditLimit = 0m)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var c = new Customer
            {
                TenantId = 1,
                Code = code,
                Name = "Test Customer",
                IsActive = true,
                CreditEnabled = creditEnabled,
                CreditLimit = creditLimit
            };
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
            var cat = new Category { TenantId = 1, Name = $"Cat_{Guid.NewGuid():N}" };
            ctx.Set<Category>().Add(cat);
            await ctx.SaveChangesAsync();
            var p = new Product { TenantId = 1, Name = name, Price = price, CategoryId = cat.Id };
            ctx.Set<Product>().Add(p);
            await ctx.SaveChangesAsync();
            id = p.Id;
        });
        return id;
    }

    private async Task<int> CreateAndSubmitOrderAsync(int customerId, int productId, int qty = 1)
    {
        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new List<CreateOrderLineDto>
            {
                new() { ProductId = productId, Quantity = qty, DiscountType = DiscountType.None, DiscountValue = 0 }
            }
        });
        await _orderService.SubmitAsync(order.Data.Id);
        return order.Data.Id;
    }

    private async Task SeedOpenInvoiceAsync(int orderId, decimal total, decimal paidAmount = 0m)
    {
        await UsingDbContextAsync(async ctx =>
        {
            var invoice = new Invoice
            {
                TenantId = 1,
                OrderId = orderId,
                InvoiceNumber = $"INV-{Guid.NewGuid():N}",
                InvoiceDate = DateTime.UtcNow,
                Status = InvoiceStatus.Issued,
                CustomerName = "Test",
                SubTotal = total,
                TaxTotal = 0m,
                DiscountAmount = 0m,
                Total = total,
                PaidAmount = paidAmount
            };
            ctx.Set<Invoice>().Add(invoice);
            await ctx.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task CreditDisabled_Order_Confirms_Regardless_Of_Balance()
    {
        var custId = await SeedCustomerAsync("CR001", creditEnabled: false, creditLimit: 100m);
        var prodId = await SeedProductAsync("Widget1", price: 200m);

        var orderId = await CreateAndSubmitOrderAsync(custId, prodId, qty: 1);

        var order = await _orderService.GetAsync(new EntityDto<int>(orderId));
        order.Data.Status.ShouldBe(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Under_Credit_Limit_Order_Confirms()
    {
        var custId = await SeedCustomerAsync("CR002", creditEnabled: true, creditLimit: 500m);
        var prodId = await SeedProductAsync("Widget2", price: 100m);

        var orderId = await CreateAndSubmitOrderAsync(custId, prodId, qty: 1);

        var order = await _orderService.GetAsync(new EntityDto<int>(orderId));
        order.Data.Status.ShouldBe(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Over_Credit_Limit_Order_Goes_To_PendingApproval()
    {
        var custId = await SeedCustomerAsync("CR003", creditEnabled: true, creditLimit: 50m);
        var prodId = await SeedProductAsync("Widget3", price: 100m);

        var orderId = await CreateAndSubmitOrderAsync(custId, prodId, qty: 1);

        var order = await _orderService.GetAsync(new EntityDto<int>(orderId));
        order.Data.Status.ShouldBe(OrderStatus.PendingApproval);
    }

    [Fact]
    public async Task Outstanding_Balance_Plus_OrderTotal_Exceeding_Limit_Goes_To_PendingApproval()
    {
        var custId = await SeedCustomerAsync("CR004", creditEnabled: true, creditLimit: 200m);
        var prodId = await SeedProductAsync("Widget4a", price: 10m);

        // Create a prior confirmed order and seed an open invoice for 150
        var priorOrderId = await CreateAndSubmitOrderAsync(custId, prodId, qty: 1);
        await SeedOpenInvoiceAsync(priorOrderId, total: 150m, paidAmount: 0m);

        // New order with price 100 — outstanding(150) + new(100) = 250 > limit(200)
        var prodId2 = await SeedProductAsync("Widget4b", price: 100m);
        var newOrderId = await CreateAndSubmitOrderAsync(custId, prodId2, qty: 1);

        var order = await _orderService.GetAsync(new EntityDto<int>(newOrderId));
        order.Data.Status.ShouldBe(OrderStatus.PendingApproval);
    }

    [Fact]
    public async Task GetCreditStatusAsync_Returns_Correct_OutstandingBalance()
    {
        var custId = await SeedCustomerAsync("CR005", creditEnabled: true, creditLimit: 300m);
        var prodId = await SeedProductAsync("Widget5", price: 10m);

        var priorOrderId = await CreateAndSubmitOrderAsync(custId, prodId, qty: 1);
        // Seed invoice: total=120, paidAmount=20 → outstanding=100
        await SeedOpenInvoiceAsync(priorOrderId, total: 120m, paidAmount: 20m);

        var status = await _customerService.GetCreditStatusAsync(custId);

        status.Data.CreditEnabled.ShouldBeTrue();
        status.Data.CreditLimit.ShouldBe(300m);
        status.Data.OutstandingBalance.ShouldBe(100m);
        status.Data.AvailableCredit.ShouldBe(200m);
    }
}
