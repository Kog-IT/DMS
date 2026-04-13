using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using DMS.Authorization;
using DMS.Routes.Dto;
using DMS.Visits;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Routes;

[AbpAuthorize(PermissionNames.Pages_Routes)]
public class RouteAppService : AsyncCrudAppService<
    Route,
    RouteDto,
    int,
    PagedRouteResultRequestDto,
    CreateRouteDto,
    UpdateRouteDto>, IRouteAppService
{
    private readonly IRepository<Visit, int> _visitRepository;

    public RouteAppService(
        IRepository<Route, int> repository,
        IRepository<Visit, int> visitRepository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Routes;
        GetAllPermissionName = PermissionNames.Pages_Routes;
        CreatePermissionName = PermissionNames.Pages_Routes_Create;
        UpdatePermissionName = PermissionNames.Pages_Routes_Edit;
        DeletePermissionName = PermissionNames.Pages_Routes_Delete;

        _visitRepository = visitRepository;
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
    public async Task ActivateAsync(int routeId)
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
    }
}
