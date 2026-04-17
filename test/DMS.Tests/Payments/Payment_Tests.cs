using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.UI;
using DMS.Categories;
using DMS.Customers;
using DMS.Invoices;
using DMS.Invoices.Dto;
using DMS.Orders;
using DMS.Orders.Dto;
using DMS.Payments;
using DMS.Payments.Dto;
using RecordPaymentDto = DMS.Payments.Dto.RecordPaymentDto;
using DMS.Products;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Payments;

public class Payment_Tests : DMSTestBase
{
    private readonly IPaymentAppService _paymentService;
    private readonly IInvoiceAppService _invoiceService;
    private readonly IOrderAppService _orderService;

    public Payment_Tests()
    {
        _paymentService = Resolve<IPaymentAppService>();
        _invoiceService = Resolve<IInvoiceAppService>();
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

    private async Task<int> SeedPaymentMethodAsync(string code, string name)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var m = new PaymentMethod { TenantId = 1, Code = code, Name = name, IsActive = true, DisplayOrder = 1 };
            ctx.Set<PaymentMethod>().Add(m);
            await ctx.SaveChangesAsync();
            id = m.Id;
        });
        return id;
    }

    private async Task<InvoiceDto> CreateIssuedInvoiceAsync(string customerCode, string productName)
    {
        var customerId = await SeedCustomerAsync(customerCode);
        var productId = await SeedProductAsync(productName, 100m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 2 } }
        });
        await _orderService.SubmitAsync(order.Id);

        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);
        await _invoiceService.IssueAsync(invoice.Id);
        return await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
    }

    [Fact]
    public async Task RecordPayment_Full_SingleMethod_Sets_Paid()
    {
        LoginAsDefaultTenantAdmin();
        var invoice = await CreateIssuedInvoiceAsync("PAY_C1", "WidgetP1");
        var methodId = await SeedPaymentMethodAsync("CASH", "Cash");

        var payment = await _paymentService.RecordPaymentAsync(new RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new() { new CreatePaymentLineDto { PaymentMethodId = methodId, Amount = invoice.Total } }
        });

        payment.ShouldNotBeNull();
        payment.ReceiptNumber.ShouldStartWith("RCP-");
        payment.TotalAmount.ShouldBe(invoice.Total);
        payment.Lines.Count.ShouldBe(1);

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.Paid);
        updated.PaidAmount.ShouldBe(invoice.Total);
    }

    [Fact]
    public async Task RecordPayment_Partial_SplitMethod_Sets_PartiallyPaid()
    {
        LoginAsDefaultTenantAdmin();
        var invoice = await CreateIssuedInvoiceAsync("PAY_C2", "WidgetP2");
        var cashId = await SeedPaymentMethodAsync("CASH2", "Cash");
        var walletId = await SeedPaymentMethodAsync("VF_CASH", "Vodafone Cash");

        var partial = invoice.Total - 50m;
        var payment = await _paymentService.RecordPaymentAsync(new RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new()
            {
                new CreatePaymentLineDto { PaymentMethodId = cashId, Amount = partial - 30m },
                new CreatePaymentLineDto { PaymentMethodId = walletId, Amount = 30m }
            }
        });

        payment.Lines.Count.ShouldBe(2);
        payment.TotalAmount.ShouldBe(partial);

        var updated = await _invoiceService.GetAsync(new EntityDto<int>(invoice.Id));
        updated.Status.ShouldBe(InvoiceStatus.PartiallyPaid);
    }

    [Fact]
    public async Task RecordPayment_Exceeds_Balance_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var invoice = await CreateIssuedInvoiceAsync("PAY_C3", "WidgetP3");
        var methodId = await SeedPaymentMethodAsync("CASH3", "Cash");

        var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _paymentService.RecordPaymentAsync(new RecordPaymentDto
            {
                InvoiceId = invoice.Id,
                PaymentDate = DateTime.UtcNow,
                Lines = new() { new CreatePaymentLineDto { PaymentMethodId = methodId, Amount = invoice.Total + 1m } }
            }));

        ex.Message.ShouldBe("Payment amount exceeds invoice balance.");
    }

    [Fact]
    public async Task RecordPayment_On_Draft_Invoice_Throws()
    {
        LoginAsDefaultTenantAdmin();
        var customerId = await SeedCustomerAsync("PAY_C4");
        var productId = await SeedProductAsync("WidgetP4", 100m);
        var methodId = await SeedPaymentMethodAsync("CASH4", "Cash");

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Lines = new() { new CreateOrderLineDto { ProductId = productId, Quantity = 1 } }
        });
        await _orderService.SubmitAsync(order.Id);
        var invoice = await _invoiceService.GenerateFromOrderAsync(order.Id);

        var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _paymentService.RecordPaymentAsync(new RecordPaymentDto
            {
                InvoiceId = invoice.Id,
                PaymentDate = DateTime.UtcNow,
                Lines = new() { new CreatePaymentLineDto { PaymentMethodId = methodId, Amount = 50m } }
            }));

        ex.Message.ShouldBe("Invoice is not in a payable state.");
    }

    [Fact]
    public async Task GetAll_Filters_By_InvoiceId()
    {
        LoginAsDefaultTenantAdmin();
        var invoice = await CreateIssuedInvoiceAsync("PAY_C5", "WidgetP5");
        var methodId = await SeedPaymentMethodAsync("CASH5", "Cash");

        await _paymentService.RecordPaymentAsync(new RecordPaymentDto
        {
            InvoiceId = invoice.Id,
            PaymentDate = DateTime.UtcNow,
            Lines = new() { new CreatePaymentLineDto { PaymentMethodId = methodId, Amount = invoice.Total } }
        });

        var result = await _paymentService.GetAllAsync(new PagedPaymentRequestDto
        {
            InvoiceId = invoice.Id,
            MaxResultCount = 10,
            SkipCount = 0
        });

        result.TotalCount.ShouldBe(1);
        result.Items[0].InvoiceId.ShouldBe(invoice.Id);
    }
}
