using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.UI;
using DMS.Customers;
using DMS.Invoices;
using DMS.Invoices.Dto;
using DMS.Orders;
using DMS.Orders.Dto;
using DMS.Payments;
using DMS.Payments.Dto;
using DMS.Products;
using DMS.Categories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Invoices;

public class Invoice_Tests : DMSTestBase
{
    private readonly IInvoiceAppService _invoiceService;
    private readonly IOrderAppService _orderService;
    private readonly IPaymentAppService _paymentService;

    public Invoice_Tests()
    {
        _invoiceService = Resolve<IInvoiceAppService>();
        _orderService = Resolve<IOrderAppService>();
        _paymentService = Resolve<IPaymentAppService>();
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

    private async Task<int> SeedPaymentMethodAsync(string code)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var m = new PaymentMethod { TenantId = 1, Code = code, Name = "Cash", IsActive = true, DisplayOrder = 1 };
            ctx.Set<PaymentMethod>().Add(m);
            await ctx.SaveChangesAsync();
            id = m.Id;
        });
        return id;
    }

    private async Task<OrderDto> CreateConfirmedOrderAsync(string customerCode, string productName)
    {
        var customerId = await SeedCustomerAsync(customerCode);
        var productId = await SeedProductAsync(productName, 100m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 2 } }
        });

        // DiscountLimitSalesRep defaults to 0 → goes straight to Confirmed
        await _orderService.SubmitAsync(order.Id);

        return await _orderService.GetAsync(new EntityDto<int>(order.Id));
    }

    [Fact]
    public async Task GenerateFromOrder_Succeeds_And_Snapshots_Data()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C1", "Widget1");

        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);

        invoice.ShouldNotBeNull();
        invoice.InvoiceNumber.ShouldNotBeNullOrEmpty();
        invoice.Status.ShouldBe(InvoiceStatus.Draft);
        invoice.OrderId.ShouldBe(order.Id);
        invoice.Lines.Count.ShouldBe(order.Lines.Count);
        invoice.CustomerName.ShouldNotBeNullOrEmpty();
        invoice.Total.ShouldBe(order.Total);
    }

    [Fact]
    public async Task GenerateFromOrder_On_Draft_Order_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("INV_C2");
        var productId = await SeedProductAsync("Widget2", 100m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });

        // Order is still Draft — do NOT submit
        var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _invoiceService.GenerateFromOrderAsync(order.Id)
        );

        ex.Message.ShouldBe("Order must be confirmed or delivered to generate an invoice.");
    }

    [Fact]
    public async Task GenerateFromOrder_When_Invoice_Exists_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C3", "Widget3");

        await _invoiceService.GenerateFromOrderAsync(order.Id);

        var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _invoiceService.GenerateFromOrderAsync(order.Id)
        );

        ex.Message.ShouldBe("An invoice already exists for this order.");
    }

    [Fact]
    public async Task Issue_Invoice_Transitions_To_Issued()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C4", "Widget4");
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);

        await _invoiceService.IssueAsync(invoice.Id);

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.Issued);
    }

    [Fact]
    public async Task RecordPayment_Partial_Sets_PartiallyPaid()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C5", "Widget5");
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);
        await _invoiceService.IssueAsync(invoice.Id);

        var partialAmount = invoice.Total - 50m;
        var methodId = await SeedPaymentMethodAsync("CASH_INV_C5");
        await _paymentService.RecordPaymentAsync(new DMS.Payments.Dto.RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new() { new DMS.Payments.Dto.CreatePaymentLineDto { PaymentMethodId = methodId, Amount = partialAmount } }
        });

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.PartiallyPaid);
        updated.PaidAmount.ShouldBe(partialAmount);
    }

    [Fact]
    public async Task RecordPayment_Full_Sets_Paid()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C6", "Widget6");
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);
        await _invoiceService.IssueAsync(invoice.Id);

        var methodId = await SeedPaymentMethodAsync("CASH_INV_C6");
        await _paymentService.RecordPaymentAsync(new DMS.Payments.Dto.RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new() { new DMS.Payments.Dto.CreatePaymentLineDto { PaymentMethodId = methodId, Amount = invoice.Total } }
        });

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.Paid);
    }

    [Fact]
    public async Task Void_Invoice_With_No_Payment_Succeeds()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C7", "Widget7");
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);

        await _invoiceService.VoidAsync(new VoidInvoiceDto
        {
            InvoiceId = invoice.Id,
            Reason = "Test void"
        });

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.Voided);
        updated.VoidReason.ShouldBe("Test void");
    }

    [Fact]
    public async Task Void_Invoice_With_Payment_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var order = await CreateConfirmedOrderAsync("INV_C8", "Widget8");
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);
        await _invoiceService.IssueAsync(invoice.Id);

        var methodId = await SeedPaymentMethodAsync("CASH_INV_C8");
        await _paymentService.RecordPaymentAsync(new DMS.Payments.Dto.RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new() { new DMS.Payments.Dto.CreatePaymentLineDto { PaymentMethodId = methodId, Amount = 50m } }
        });

        var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _invoiceService.VoidAsync(new VoidInvoiceDto
            {
                InvoiceId = invoice.Id,
                Reason = "Should fail"
            })
        );

        ex.Message.ShouldBe("Cannot void an invoice with recorded payments.");
    }
}
