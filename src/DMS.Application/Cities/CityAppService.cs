using System.Collections.Generic;
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
        public CityAppService(IRepository<City, int> repository) : base(repository) { }

        protected override IQueryable<City> CreateFilteredQuery(PagedCityResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Governorate)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Name.Contains(input.Keyword) || x.Name_EN.Contains(input.Keyword))
                .WhereIf(input.GovernorateId.HasValue, x => x.GovernorateId == input.GovernorateId.Value);
        }

        public override async Task<ApiResponse<CityDto>> GetAsync(EntityDto<int> input)
        {
            var city = await Repository.GetAllIncluding(x => x.Governorate)
                                       .FirstOrDefaultAsync(x => x.Id == input.Id);
            if (city == null)
                throw new Abp.UI.UserFriendlyException("المدينة غير موجودة!");
            return Ok(MapToEntityDto(city), L("RetrievedSuccessfully"));
        }

        public async Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("DeletedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                await Repository.DeleteAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("DeletedSuccessfully"));
        }

        public async Task<ApiResponse<object>> BulkActivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }
    }
}
