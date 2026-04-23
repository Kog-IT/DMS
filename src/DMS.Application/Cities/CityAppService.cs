using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Cities.Dto;
using DMS.Common;
using DMS.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace DMS.Cities
{
    public class CityAppService : DmsCrudAppService<City, CityDto, int, PagedCityResultRequestDto, CreateCityDto, UpdateCityDto>, ICityAppService
    {
        public CityAppService(IRepository<City, int> repository) : base(repository)
        {
        }

        protected override IQueryable<City> CreateFilteredQuery(PagedCityResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Governorate)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Name_EN.Contains(input.Keyword))
                .WhereIf(input.GovernorateId.HasValue, x => x.GovernorateId == input.GovernorateId.Value);
        }

        public override async Task<ApiResponse<CityDto>> GetAsync(EntityDto<int> input)
        {
            var city = await Repository.GetAllIncluding(x => x.Governorate)
                                       .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (city == null)
            {
                throw new Abp.UI.UserFriendlyException("المدينة غير موجودة!");
            }

            var dto = MapToEntityDto(city);
            return Ok(dto, L("RetrievedSuccessfully"));
        }
    }
}
