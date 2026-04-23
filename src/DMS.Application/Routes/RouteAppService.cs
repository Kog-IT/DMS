using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Customers;
using DMS.Routes.Dto;
using DMS.Visits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Routes;

[AbpAuthorize(PermissionNames.Pages_Routes)]
public class RouteAppService : DmsCrudAppService<
    Route,
    RouteDto,
    int,
    PagedRouteResultRequestDto,
    CreateRouteDto,
    UpdateRouteDto>, IRouteAppService
{
    private readonly IRepository<Visit, int> _visitRepository;
    private readonly IRepository<RouteItem, int> _routeItemRepository;
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly ISettingManager _settingManager;
    private readonly RouteOptimizationService _optimizationService;

    public RouteAppService(
        IRepository<Route, int> repository,
        IRepository<Visit, int> visitRepository,
        IRepository<RouteItem, int> routeItemRepository,
        IRepository<Customer, int> customerRepository,
        ISettingManager settingManager,
        RouteOptimizationService optimizationService)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Routes;
        GetAllPermissionName = PermissionNames.Pages_Routes;
        CreatePermissionName = PermissionNames.Pages_Routes_Create;
        UpdatePermissionName = PermissionNames.Pages_Routes_Edit;
        DeletePermissionName = PermissionNames.Pages_Routes_Delete;

        _visitRepository = visitRepository;
        _routeItemRepository = routeItemRepository;
        _customerRepository = customerRepository;
        _settingManager = settingManager;
        _optimizationService = optimizationService;
    }

    protected override IQueryable<Route> CreateFilteredQuery(PagedRouteResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), r => r.Name.Contains(input.Keyword))
            .WhereIf(input.AssignedUserId.HasValue, r => r.AssignedUserId == input.AssignedUserId.Value)
            .WhereIf(input.Status.HasValue, r => r.Status == input.Status.Value)
            .WhereIf(input.DateFrom.HasValue, r => r.PlannedDate >= input.DateFrom.Value)
            .WhereIf(input.DateTo.HasValue, r => r.PlannedDate <= input.DateTo.Value);
    }

    [AbpAuthorize(PermissionNames.Pages_Routes_Edit)]
    public async Task<ApiResponse<object>> ActivateAsync(int routeId)
    {
        var route = await Repository.GetAll()
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == routeId);

        if (route == null)
            throw new UserFriendlyException("Route not found.");

        if (route.Status != RouteStatus.Draft)
            throw new UserFriendlyException("Only draft routes can be activated.");

        route.Status = RouteStatus.Active;
        await Repository.UpdateAsync(route);

        foreach (var item in route.Items.OrderBy(i => i.OrderIndex))
        {
            await _visitRepository.InsertAsync(new Visit
            {
                TenantId = route.TenantId,
                RouteId = route.Id,
                RouteItemId = item.Id,
                CustomerId = item.CustomerId,
                AssignedUserId = route.AssignedUserId,
                Status = VisitStatus.Planned,
                PlannedDate = route.PlannedDate
            });
        }
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Routes_Edit)]
    public async Task<ApiResponse<OptimizeRouteResultDto>> OptimizeRouteAsync(OptimizeRouteInputDto input)
    {
        var route = await Repository.GetAll()
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == input.RouteId);

        if (route == null)
            throw new UserFriendlyException("Route not found.");

        if (route.Status == RouteStatus.Completed)
            throw new UserFriendlyException("Cannot optimize a completed route.");

        if (!route.Items.Any())
            return Ok(new OptimizeRouteResultDto { RouteId = input.RouteId }, L("RetrievedSuccessfully"));

        // Load customer GPS data
        var customerIds = route.Items.Select(i => i.CustomerId).Distinct().ToList();
        var customers = await _customerRepository.GetAll()
            .Where(c => customerIds.Contains(c.Id))
            .ToListAsync();
        var customerMap = customers.ToDictionary(c => c.Id);

        // Read tenant settings
        var defaultDuration = await _settingManager.GetSettingValueAsync<int>(
            VisitSettingNames.DefaultVisitDurationMinutes);
        var travelSpeed = await _settingManager.GetSettingValueAsync<double>(
            VisitSettingNames.AverageTravelSpeedKmh);

        var startTime = input.StartTime ?? Clock.Now;

        // Build stop inputs for domain service
        var stopInputs = route.Items.Select(item =>
        {
            customerMap.TryGetValue(item.CustomerId, out var customer);
            return new RouteOptimizationService.StopInput
            {
                RouteItemId = item.Id,
                CustomerId = item.CustomerId,
                CustomerName = customer?.Name,
                CustomerAddress = customer?.Address,
                Latitude = customer?.Latitude,
                Longitude = customer?.Longitude,
                PlannedDurationMinutes = item.PlannedDurationMinutes
            };
        }).ToList();

        // Run optimization
        var optimized = _optimizationService.Optimize(
            stopInputs,
            input.RepLatitude,
            input.RepLongitude,
            startTime,
            defaultDuration,
            travelSpeed);

        // Persist updated OrderIndex values
        foreach (var result in optimized)
        {
            var item = route.Items.First(i => i.Id == result.RouteItemId);
            item.OrderIndex = result.OrderIndex;
            await _routeItemRepository.UpdateAsync(item);
        }

        // Build result DTO
        var resultItems = optimized.Select(r => new OptimizedRouteItemDto
        {
            RouteItemId = r.RouteItemId,
            OrderIndex = r.OrderIndex,
            CustomerId = r.CustomerId,
            CustomerName = r.CustomerName,
            CustomerAddress = r.CustomerAddress,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            DistanceFromPreviousKm = r.DistanceFromPreviousKm,
            PlannedDurationMinutes = r.PlannedDurationMinutes,
            EstimatedArrivalTime = r.EstimatedArrivalTime
        }).ToList();

        var totalDistanceKm = resultItems.Sum(i => i.DistanceFromPreviousKm);
        var totalDurationMinutes = resultItems.Sum(i => i.PlannedDurationMinutes)
            + (int)resultItems.Sum(i =>
                i.DistanceFromPreviousKm > 0 && travelSpeed > 0
                    ? (i.DistanceFromPreviousKm / travelSpeed) * 60
                    : 0);

        return Ok(new OptimizeRouteResultDto
        {
            RouteId = input.RouteId,
            Items = resultItems,
            TotalDistanceKm = Math.Round(totalDistanceKm, 3),
            TotalDurationMinutes = totalDurationMinutes,
            EstimatedEndTime = startTime.AddMinutes(totalDurationMinutes)
        }, L("RetrievedSuccessfully"));
    }
}
