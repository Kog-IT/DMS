using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using DMS.Authorization;
using DMS.Customers;
using DMS.Invoices.Dto;
using DMS.Orders;
using Microsoft.EntityFrameworkCore;

namespace DMS.Invoices;

[AbpAuthorize(PermissionNames.Pages_Invoices)]
public class InvoiceAppService : AsyncCrudAppService<
    Invoice,
    InvoiceDto,
    int,
    PagedInvoiceResultRequestDto,
    GenerateInvoiceDto,
    GenerateInvoiceDto>, IInvoiceAppService
{
    private readonly IRepository<InvoiceLine, int> _lineRepository;
    private readonly IRepository<Order, int> _orderRepository;
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly ISettingManager _settingManager;
    private readonly InvoiceNumberGenerator _invoiceNumberGenerator;

    public InvoiceAppService(
        IRepository<Invoice, int> repository,
        IRepository<InvoiceLine, int> lineRepository,
        IRepository<Order, int> orderRepository,
        IRepository<Customer, int> customerRepository,
        ISettingManager settingManager,
        InvoiceNumberGenerator invoiceNumberGenerator)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Invoices;
        GetAllPermissionName = PermissionNames.Pages_Invoices;
        CreatePermissionName = PermissionNames.Pages_Invoices_Create;
        UpdatePermissionName = PermissionNames.Pages_Invoices_Edit;
        DeletePermissionName = PermissionNames.Pages_Invoices_Delete;

        _lineRepository = lineRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _settingManager = settingManager;
        _invoiceNumberGenerator = invoiceNumberGenerator;
    }

    protected override IQueryable<Invoice> CreateFilteredQuery(PagedInvoiceResultRequestDto input)
    {
        IQueryable<Invoice> query = Repository.GetAll().Include(i => i.Lines);

        if (input.CustomerId.HasValue)
        {
            var orderIds = _orderRepository.GetAll()
                .Where(o => o.CustomerId == input.CustomerId.Value)
                .Select(o => o.Id);
            query = query.Where(i => orderIds.Contains(i.OrderId));
        }

        if (input.Status.HasValue)
            query = query.Where(i => i.Status == input.Status.Value);

        if (input.FromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= input.FromDate.Value);

        if (input.ToDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= input.ToDate.Value);

        return query;
    }

    protected override async Task<Invoice> GetEntityByIdAsync(int id)
    {
        return await Repository.GetAll()
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new UserFriendlyException("Invoice not found.");
    }

    public async Task<InvoiceDto> GenerateFromOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetAll()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new UserFriendlyException("Order not found.");

        if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.Delivered)
            throw new UserFriendlyException("Order must be confirmed or delivered to generate an invoice.");

        var alreadyExists = await Repository.GetAll().AnyAsync(i => i.OrderId == orderId);
        if (alreadyExists)
            throw new UserFriendlyException("An invoice already exists for this order.");

        var customer = await _customerRepository.GetAll()
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

        var invoiceNumber = await _invoiceNumberGenerator.GenerateAsync(AbpSession.TenantId!.Value);

        var dueDaysStr = await _settingManager.GetSettingValueAsync(InvoiceSettingNames.DueDaysDefault);
        var dueDays = int.Parse(dueDaysStr);

        var invoice = new Invoice
        {
            TenantId = AbpSession.TenantId!.Value,
            OrderId = order.Id,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(dueDays),
            Status = InvoiceStatus.Draft,
            CustomerName = customer?.Name ?? string.Empty,
            CustomerAddress = customer?.Address ?? string.Empty,
            SubTotal = order.SubTotal,
            TaxTotal = order.TaxTotal,
            DiscountAmount = order.OrderDiscountAmount,
            Total = order.Total,
            PaidAmount = 0
        };

        await Repository.InsertAsync(invoice);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var ol in order.Lines)
        {
            var line = new InvoiceLine
            {
                TenantId = invoice.TenantId,
                InvoiceId = invoice.Id,
                ProductName = ol.ProductName,
                UnitPrice = ol.UnitPrice,
                TaxRate = ol.TaxRate,
                Quantity = ol.Quantity,
                DiscountType = ol.DiscountType,
                DiscountValue = ol.DiscountValue,
                LineTotal = ol.LineTotal
            };
            await _lineRepository.InsertAsync(line);
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return await GetAsync(new EntityDto<int>(invoice.Id));
    }

    public async Task IssueAsync(int id)
    {
        var invoice = await GetEntityByIdAsync(id);

        if (invoice.Status != InvoiceStatus.Draft)
            throw new UserFriendlyException("Only draft invoices can be issued.");

        invoice.Status = InvoiceStatus.Issued;
        await Repository.UpdateAsync(invoice);
    }

    public async Task VoidAsync(VoidInvoiceDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Invoices_Void);

        var invoice = await GetEntityByIdAsync(input.InvoiceId);

        if (invoice.Status != InvoiceStatus.Draft &&
            invoice.Status != InvoiceStatus.Issued &&
            invoice.Status != InvoiceStatus.PartiallyPaid)
            throw new UserFriendlyException("Invoice cannot be voided in its current state.");

        if (invoice.PaidAmount != 0)
            throw new UserFriendlyException("Cannot void an invoice with recorded payments.");

        invoice.VoidReason = input.Reason;
        invoice.Status = InvoiceStatus.Voided;
        await Repository.UpdateAsync(invoice);
    }
}
