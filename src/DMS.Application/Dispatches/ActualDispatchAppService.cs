using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Dispatches.Dto;
using System.Linq;

namespace DMS.Dispatches;

public class ActualDispatchAppService : DmsCrudAppService<
    ActualDispatch, ActualDispatchDto, int,
    PagedActualDispatchResultRequestDto,
    CreateActualDispatchDto, UpdateActualDispatchDto>, IActualDispatchAppService
{
    public ActualDispatchAppService(IRepository<ActualDispatch, int> repository) : base(repository) { }

    protected override IQueryable<ActualDispatch> CreateFilteredQuery(PagedActualDispatchResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(input.SalesmanId.HasValue, x => x.SalesmanId == input.SalesmanId.Value)
            .WhereIf(input.DateFrom.HasValue, x => x.DispatchDate >= input.DateFrom.Value)
            .WhereIf(input.DateTo.HasValue, x => x.DispatchDate <= input.DateTo.Value);
    }
}
