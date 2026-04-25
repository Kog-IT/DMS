using System.Linq;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Governorates.Dto;

namespace DMS.Governorates
{
    public class GovernorateAppService : DmsCrudAppService<Governorate, GovernorateDto, int, PagedGovernorateResultRequestDto, CreateGovernorateDto, UpdateGovernorateDto>, IGovernorateAppService
    {
        public GovernorateAppService(IRepository<Governorate, int> repository) : base(repository)
        {
        }

        protected override IQueryable<Governorate> CreateFilteredQuery(PagedGovernorateResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(
                 !input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Name_EN.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }
    }
}
