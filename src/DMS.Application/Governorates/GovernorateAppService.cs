using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Governorates.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Governorates
{
    public class GovernorateAppService : DmsCrudAppService<Governorate, GovernorateDto, int, PagedGovernorateResultRequestDto, CreateGovernorateDto, UpdateGovernorateDto>, IGovernorateAppService
    {
        private readonly IRepository<DMS.Media.MediaFile, int> _mediaRepository;

        public GovernorateAppService(
            IRepository<Governorate, int> repository,
            IRepository<DMS.Media.MediaFile, int> mediaRepository)
            : base(repository)
        {
            _mediaRepository = mediaRepository;
        }

        protected override GovernorateDto MapToEntityDto(Governorate entity)
        {
            var dto = base.MapToEntityDto(entity);
            dto.Media = _mediaRepository.GetAll()
                .Where(m => m.MediaType == DMS.Media.MediaType.Governorate && m.ModelId == entity.Id)
                .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
                .ToList();
            return dto;
        }

        protected override async Task<Governorate> GetEntityByIdAsync(int id)
            => await Repository.GetAll()
                .Include(g => g.Cities)
                .FirstOrDefaultAsync(g => g.Id == id)
                ?? throw new Abp.UI.UserFriendlyException("Governorate not found.");

        protected override IQueryable<Governorate> CreateFilteredQuery(PagedGovernorateResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(g => g.Cities)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Name.Contains(input.Keyword) || x.Name_EN.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
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

        [HttpPost]
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
