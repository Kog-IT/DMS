using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Brands.Dto;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Media;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Brands;

public class BrandAppService : DmsCrudAppService<
    Brand, BrandDto, int,
    PagedBrandResultRequestDto,
    CreateBrandDto, UpdateBrandDto>, IBrandAppService
{
    private readonly IRepository<MediaFile, int> _mediaRepository;

    public BrandAppService(
        IRepository<Brand, int> repository,
        IRepository<MediaFile, int> mediaRepository)
        : base(repository)
    {
        _mediaRepository = mediaRepository;
    }

    public override async Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto input)
    {
        var result = await base.CreateAsync(input);
        var brandId = result.Data.Id;

        if (input.Paths != null)
        {
            foreach (var path in input.Paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                await _mediaRepository.InsertAsync(new MediaFile
                {
                    TenantId = AbpSession.TenantId ?? 0,
                    MediaType = MediaType.Brand,
                    ModelId = brandId,
                    FilePath = path,
                    FileName = System.IO.Path.GetFileName(path),
                    ContentType = GetContentType(path)
                });
            }
            if (input.Paths.Count > 0)
                await CurrentUnitOfWork.SaveChangesAsync();
        }

        return await GetAsync(new Abp.Application.Services.Dto.EntityDto<int>(brandId));
    }

    public override async Task<ApiResponse<BrandDto>> UpdateAsync(UpdateBrandDto input)
    {
        await base.UpdateAsync(input);

        if (input.Paths != null)
        {
            var tenantId = AbpSession.TenantId ?? 0;

            // Delete existing media for this brand
            var existing = _mediaRepository.GetAll()
                .Where(m => m.MediaType == MediaType.Brand && m.ModelId == input.Id)
                .ToList();
            foreach (var m in existing)
                await _mediaRepository.DeleteAsync(m);

            // Insert only the current set of paths
            foreach (var path in input.Paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                await _mediaRepository.InsertAsync(new MediaFile
                {
                    TenantId = tenantId,
                    MediaType = MediaType.Brand,
                    ModelId = input.Id,
                    FilePath = path,
                    FileName = System.IO.Path.GetFileName(path),
                    ContentType = GetContentType(path)
                });
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return await GetAsync(new Abp.Application.Services.Dto.EntityDto<int>(input.Id));
    }

    private static string GetContentType(string path)
    {
        var ext = System.IO.Path.GetExtension(path)?.ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }

    protected override BrandDto MapToEntityDto(Brand entity)
    {
        var dto = base.MapToEntityDto(entity);
        dto.Media = _mediaRepository.GetAll()
            .Where(m => m.MediaType == MediaType.Brand && m.ModelId == entity.Id)
            .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
            .ToList();
        return dto;
    }

    protected override IQueryable<Brand> CreateFilteredQuery(PagedBrandResultRequestDto input)
    {
        return Repository.GetAll()
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
