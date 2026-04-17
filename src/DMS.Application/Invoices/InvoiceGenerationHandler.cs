using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using DMS.Orders;
using Microsoft.EntityFrameworkCore;

namespace DMS.Invoices;

public class InvoiceGenerationHandler :
    IAsyncEventHandler<EntityUpdatedEventData<Order>>,
    ITransientDependency
{
    private readonly ISettingManager _settingManager;
    private readonly IRepository<Invoice, int> _invoiceRepository;
    private readonly IInvoiceAppService _invoiceAppService;

    public InvoiceGenerationHandler(
        ISettingManager settingManager,
        IRepository<Invoice, int> invoiceRepository,
        IInvoiceAppService invoiceAppService)
    {
        _settingManager = settingManager;
        _invoiceRepository = invoiceRepository;
        _invoiceAppService = invoiceAppService;
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<Order> eventData)
    {
        var trigger = await _settingManager.GetSettingValueAsync(InvoiceSettingNames.AutoGenerateTrigger);
        var order = eventData.Entity;

        bool shouldGenerate =
            (trigger == "OnConfirmed" && order.Status == OrderStatus.Confirmed) ||
            (trigger == "OnDelivered" && order.Status == OrderStatus.Delivered);

        if (!shouldGenerate) return;

        var exists = await _invoiceRepository.GetAll().AnyAsync(i => i.OrderId == order.Id);
        if (exists) return;

        await _invoiceAppService.GenerateFromOrderAsync(order.Id);
    }
}
