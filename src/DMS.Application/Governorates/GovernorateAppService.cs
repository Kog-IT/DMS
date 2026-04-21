using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Governorates.Dto;

namespace DMS.Governorates
{
    public class GovernorateAppService : AsyncCrudAppService<Governorate, GovernorateDto, int, PagedGovernorateResultRequestDto, CreateGovernorateDto, UpdateGovernorateDto>, IGovernorateAppService
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
