using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using DMS.Authorization;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Customers;
using DMS.Orders.Dto;
using DMS.PriceLists;
using DMS.Products;
using Microsoft.EntityFrameworkCore;

namespace DMS.Orders;

[AbpAuthorize(PermissionNames.Pages_Orders)]
public class OrderAppService : DmsCrudAppService<
    Order,
    OrderDto,
    int,
    PagedOrderResultRequestDto,
    CreateOrderDto,
    UpdateOrderDto>, IOrderAppService
{
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IRepository<OrderLine, int> _lineRepository;
    private readonly ISettingManager _settingManager;
    private readonly UserManager _userManager;
    private readonly PriceResolutionService _priceResolutionService;
    private readonly CreditCheckService _creditCheckService;

    public OrderAppService(
        IRepository<Order, int> repository,
        IRepository<Customer, int> customerRepository,
        IRepository<Product, int> productRepository,
        IRepository<OrderLine, int> lineRepository,
        ISettingManager settingManager,
        UserManager userManager,
        PriceResolutionService priceResolutionService,
        CreditCheckService creditCheckService)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Orders;
        GetAllPermissionName = PermissionNames.Pages_Orders;
        CreatePermissionName = PermissionNames.Pages_Orders_Create;
        UpdatePermissionName = PermissionNames.Pages_Orders_Edit;
        DeletePermissionName = PermissionNames.Pages_Orders_Delete;

        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _lineRepository = lineRepository;
        _settingManager = settingManager;
        _userManager = userManager;
        _priceResolutionService = priceResolutionService;
        _creditCheckService = creditCheckService;
    }

    protected override IQueryable<Order> CreateFilteredQuery(PagedOrderResultRequestDto input)
    {
        IQueryable<Order> query = Repository.GetAll().Include(o => o.Lines);

        if (input.CustomerId.HasValue)
            query = query.Where(o => o.CustomerId == input.CustomerId.Value);

        if (input.Status.HasValue)
            query = query.Where(o => o.Status == input.Status.Value);

        if (input.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= input.FromDate.Value);

        if (input.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= input.ToDate.Value);

        return query;
    }

    protected async Task<Order> GetEntityByIdAsync(int id)
    {
        return await Repository.GetAll()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id)
            ?? throw new UserFriendlyException("Order not found.");
    }

    public override async Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto input)
    {
        if (input.Lines == null || input.Lines.Count == 0)
            throw new UserFriendlyException("Order must have at least one line.");

        var customer = await _customerRepository.GetAll()
            .FirstOrDefaultAsync(c => c.Id == input.CustomerId);
        if (customer == null)
            throw new UserFriendlyException("Customer not found.");
        if (customer.IsBlocked)
            throw new UserFriendlyException("Customer is blocked. Orders cannot be placed for a blocked customer.");

        var allowBackOrder = await _settingManager.GetSettingValueAsync(OrderSettingNames.AllowOrdersWithoutStock);
        var backOrderAllowed = bool.Parse(allowBackOrder);

        var order = ObjectMapper.Map<Order>(input);
        order.AssignedUserId = AbpSession.UserId!.Value;
        order.Status = OrderStatus.Draft;
        order.TenantId = AbpSession.TenantId!.Value;

        var lines = await BuildLinesAsync(input.Lines, backOrderAllowed, order.TenantId, order.CustomerId);
        ComputeTotals(order, lines);

        await Repository.InsertAsync(order);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var line in lines)
        {
            line.OrderId = order.Id;
            line.TenantId = order.TenantId;
            await _lineRepository.InsertAsync(line);
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return await GetAsync(new EntityDto<int>(order.Id));
    }

    public override async Task<ApiResponse<OrderDto>> UpdateAsync(UpdateOrderDto input)
    {
        var order = await GetEntityByIdAsync(input.Id);

        if (order.Status != OrderStatus.Draft)
            throw new UserFriendlyException("Only draft orders can be edited.");

        if (input.Lines == null || input.Lines.Count == 0)
            throw new UserFriendlyException("Order must have at least one line.");

        var allowBackOrder = await _settingManager.GetSettingValueAsync(OrderSettingNames.AllowOrdersWithoutStock);
        var backOrderAllowed = bool.Parse(allowBackOrder);

        ObjectMapper.Map(input, order);

        foreach (var existing in order.Lines.ToList())
            await _lineRepository.DeleteAsync(existing);

        var lines = await BuildLinesAsync(input.Lines, backOrderAllowed, order.TenantId, order.CustomerId);
        ComputeTotals(order, lines);

        await Repository.UpdateAsync(order);
        await CurrentUnitOfWork.SaveChangesAsync();

        foreach (var line in lines)
        {
            line.OrderId = order.Id;
            line.TenantId = order.TenantId;
            await _lineRepository.InsertAsync(line);
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return await GetAsync(new EntityDto<int>(order.Id));
    }

    private async Task<List<OrderLine>> BuildLinesAsync(
        List<CreateOrderLineDto> inputLines,
        bool backOrderAllowed,
        int tenantId,
        int customerId)
    {
        var lines = new List<OrderLine>();

        foreach (var inputLine in inputLines)
        {
            var product = await _productRepository.GetAll()
                .FirstOrDefaultAsync(p => p.Id == inputLine.ProductId);

            if (product == null)
                throw new UserFriendlyException("Product not found.");

            if (!backOrderAllowed)
                throw new UserFriendlyException($"Insufficient stock for product '{product.Name}'.");

            var resolution = await _priceResolutionService.ResolveAsync(
                customerId,
                product.Id,
                inputLine.Quantity);

            var line = new OrderLine
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = resolution.Price,
                TaxRate = product.TaxRate,
                Quantity = inputLine.Quantity,
                DiscountType = inputLine.DiscountType,
                DiscountValue = inputLine.DiscountValue,
                IsBackOrder = false,
                IsBasePriceFallback = resolution.IsBasePriceFallback
            };

            if (line.DiscountType == DiscountType.Percentage && line.DiscountValue > 100)
                throw new UserFriendlyException("Line discount percentage cannot exceed 100.");

            line.LineTotal = ComputeLineTotal(line);
            lines.Add(line);
        }

        return lines;
    }

    private static decimal CalculateLineDiscount(OrderLine line)
    {
        var baseAmount = line.UnitPrice * line.Quantity;
        return line.DiscountType switch
        {
            DiscountType.Percentage => baseAmount * (line.DiscountValue / 100m),
            DiscountType.Amount => line.DiscountValue,
            _ => 0m
        };
    }

    private static decimal ComputeLineTotal(OrderLine line)
    {
        var baseAmount = line.UnitPrice * line.Quantity;
        var lineDiscount = CalculateLineDiscount(line);
        var discountedBase = baseAmount - lineDiscount;
        var tax = discountedBase * (line.TaxRate / 100m);
        return discountedBase + tax;
    }

    private static void ComputeTotals(Order order, List<OrderLine> lines)
    {
        order.SubTotal = lines.Sum(l => l.UnitPrice * l.Quantity);
        order.TaxTotal = lines.Sum(l =>
        {
            var baseAmount = l.UnitPrice * l.Quantity;
            var lineDiscount = CalculateLineDiscount(l);
            return (baseAmount - lineDiscount) * (l.TaxRate / 100m);
        });

        order.OrderDiscountAmount = order.OrderDiscountType switch
        {
            DiscountType.Percentage => order.SubTotal * (order.OrderDiscountValue / 100m),
            DiscountType.Amount => order.OrderDiscountValue,
            _ => 0m
        };

        order.Total = lines.Sum(l => l.LineTotal) - order.OrderDiscountAmount;
    }

    public async Task<ApiResponse<object>> SubmitAsync(int id)
    {
        var order = await GetEntityByIdAsync(id);
        if (order.Status != OrderStatus.Draft)
            throw new UserFriendlyException("Only draft orders can be submitted.");

        var discountLimit = await GetDiscountLimitForCurrentUserAsync();

        bool discountExceedsLimit = discountLimit > 0 && (
            order.Lines.Any(l => l.DiscountValue > discountLimit) ||
            order.OrderDiscountValue > discountLimit);

        var creditResult = await _creditCheckService.CheckCreditAsync(order.CustomerId, order.Total);

        bool exceedsLimit = discountExceedsLimit || creditResult.IsOverLimit;

        order.Status = exceedsLimit ? OrderStatus.PendingApproval : OrderStatus.Confirmed;
        await Repository.UpdateAsync(order);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Orders_Approve)]
    public async Task<ApiResponse<object>> ApproveAsync(int id)
    {
        var order = await GetEntityByIdAsync(id);
        if (order.Status != OrderStatus.PendingApproval)
            throw new UserFriendlyException("Only orders pending approval can be approved.");

        order.Status = OrderStatus.Confirmed;
        await Repository.UpdateAsync(order);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Orders_Approve)]
    public async Task<ApiResponse<object>> RejectAsync(int id, string reason)
    {
        var order = await GetEntityByIdAsync(id);
        if (order.Status != OrderStatus.PendingApproval)
            throw new UserFriendlyException("Only orders pending approval can be rejected.");

        order.RejectionReason = reason;
        order.Status = OrderStatus.Cancelled;
        await Repository.UpdateAsync(order);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> CancelAsync(int id)
    {
        var order = await GetEntityByIdAsync(id);
        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Confirmed)
            throw new UserFriendlyException("Order cannot be cancelled in its current state.");

        order.Status = OrderStatus.Cancelled;
        await Repository.UpdateAsync(order);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> MarkDeliveredAsync(int id)
    {
        var order = await GetEntityByIdAsync(id);
        if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.PartiallyDelivered)
            throw new UserFriendlyException("Only confirmed or partially delivered orders can be marked as delivered.");

        order.Status = OrderStatus.Delivered;
        await Repository.UpdateAsync(order);
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    private async Task<decimal> GetDiscountLimitForCurrentUserAsync()
    {
        var userId = AbpSession.UserId;
        if (userId == null) return 0;

        var user = await _userManager.GetUserByIdAsync(userId.Value);
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(StaticRoleNames.Tenants.Supervisor))
        {
            var val = await _settingManager.GetSettingValueAsync(OrderSettingNames.DiscountLimitSupervisor);
            return decimal.Parse(val);
        }

        var salesVal = await _settingManager.GetSettingValueAsync(OrderSettingNames.DiscountLimitSalesRep);
        return decimal.Parse(salesVal);
    }
}
