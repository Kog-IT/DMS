using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Dispatches.Dto;
using System.Linq;

namespace DMS.Dispatches;

public class PlannedDispatchAppService : DmsCrudAppService<
    PlannedDispatch, PlannedDispatchDto, int,
    PagedPlannedDispatchResultRequestDto,
    CreatePlannedDispatchDto, UpdatePlannedDispatchDto>, IPlannedDispatchAppService
{
    public PlannedDispatchAppService(IRepository<PlannedDispatch, int> repository) : base(repository) { }

    protected override IQueryable<PlannedDispatch> CreateFilteredQuery(PagedPlannedDispatchResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(input.SalesmanId.HasValue, x => x.SalesmanId == input.SalesmanId.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value)
            .WhereIf(input.DateFrom.HasValue, x => x.DispatchDate >= input.DateFrom.Value)
            .WhereIf(input.DateTo.HasValue, x => x.DispatchDate <= input.DateTo.Value);
    }
}
