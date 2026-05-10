using Microsoft.AspNetCore.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using DMS.Authorization.Users;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Salesmen.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Salesmen;

public class SalesmanAppService : DmsCrudAppService<
    Salesman, SalesmanDto, int,
    PagedSalesmanResultRequestDto,
    CreateSalesmanDto, UpdateSalesmanDto>, ISalesmanAppService
{
    private readonly IRepository<SalesmanWarehouse, int> _salesmanWarehouseRepository;
    private readonly IRepository<DMS.Media.MediaFile, int> _mediaRepository;
    private readonly UserManager _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public SalesmanAppService(
        IRepository<Salesman, int> repository,
        IRepository<SalesmanWarehouse, int> salesmanWarehouseRepository,
        IRepository<DMS.Media.MediaFile, int> mediaRepository,
        UserManager userManager,
        IPasswordHasher<User> passwordHasher)
        : base(repository)
    {
        _salesmanWarehouseRepository = salesmanWarehouseRepository;
        _mediaRepository = mediaRepository;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    protected override SalesmanDto MapToEntityDto(Salesman entity)
    {
        var dto = base.MapToEntityDto(entity);
        dto.Media = _mediaRepository.GetAll()
            .Where(m => m.MediaType == DMS.Media.MediaType.Salesman && m.ModelId == entity.Id)
            .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
            .ToList();
        return dto;
    }

    protected override IQueryable<Salesman> CreateFilteredQuery(PagedSalesmanResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Keyword)
                  || x.JobCode.Contains(input.Keyword)
                  || x.Mobile.Contains(input.Keyword))
            .WhereIf(!input.NationalNumber.IsNullOrWhiteSpace(),
                x => x.NationalNumber.Contains(input.NationalNumber))
            .WhereIf(!input.Mobile.IsNullOrWhiteSpace(),
                x => x.Mobile.Contains(input.Mobile))
            .WhereIf(input.WarehouseId.HasValue,
                x => _salesmanWarehouseRepository.GetAll().Any(sw => sw.SalesmanId == x.Id && sw.WarehouseId == input.WarehouseId.Value))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
    }

    public override async Task<ApiResponse<SalesmanDto>> GetAsync(EntityDto<int> input)
    {
        var entity = await Repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<SalesmanDto>(entity);
        dto.WarehouseIds = await _salesmanWarehouseRepository.GetAll()
            .Where(sw => sw.SalesmanId == input.Id)
            .Select(sw => sw.WarehouseId)
            .ToListAsync();
        return Ok(dto, L("RetrievedSuccessfully"));
    }

    public override async Task<ApiResponse<SalesmanDto>> CreateAsync(CreateSalesmanDto input)
    {
        if (string.IsNullOrWhiteSpace(input.UserName))
            throw new UserFriendlyException("Username is required.");
        if (string.IsNullOrWhiteSpace(input.Password))
            throw new UserFriendlyException("Password is required.");

        // Create the linked ABP user
        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

        var user = new User
        {
            TenantId     = AbpSession.TenantId,
            UserName     = input.UserName,
            Name         = input.Name ?? input.UserName,
            Surname      = input.Name ?? input.UserName,
            EmailAddress = input.Email ?? $"{input.UserName}@salesman.local",
            IsActive     = input.IsActive,
            IsEmailConfirmed = true,
        };

        var createResult = await _userManager.CreateAsync(user, input.Password);
        if (!createResult.Succeeded)
            throw new UserFriendlyException(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        // Create the salesman and link to the user
        var entity = ObjectMapper.Map<Salesman>(input);
        entity.TenantId = AbpSession.TenantId ?? 1;
        entity.UserId   = user.Id;
        await Repository.InsertAsync(entity);
        await CurrentUnitOfWork.SaveChangesAsync();

        await SyncWarehousesAsync(entity.Id, input.WarehouseIds);

        var dto = ObjectMapper.Map<SalesmanDto>(entity);
        dto.WarehouseIds = input.WarehouseIds ?? new();
        return Ok(dto, L("CreatedSuccessfully"));
    }

    public override async Task<ApiResponse<SalesmanDto>> UpdateAsync(UpdateSalesmanDto input)
    {
        var entity = await Repository.GetAsync(input.Id);
        ObjectMapper.Map(input, entity);

        // Sync username back to linked ABP user if exists
        if (entity.UserId.HasValue && !string.IsNullOrWhiteSpace(input.UserName))
        {
            var user = await _userManager.FindByIdAsync(entity.UserId.Value.ToString());
            if (user != null)
            {
                user.UserName = input.UserName;
                user.Name     = input.Name ?? user.Name;
                user.Surname  = input.Name ?? user.Surname;
                if (!string.IsNullOrWhiteSpace(input.Email))
                    user.EmailAddress = input.Email;
                user.IsActive = input.IsActive;
                await _userManager.UpdateAsync(user);
            }
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        await SyncWarehousesAsync(entity.Id, input.WarehouseIds);

        var dto = ObjectMapper.Map<SalesmanDto>(entity);
        dto.WarehouseIds = input.WarehouseIds ?? new();
        return Ok(dto, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input)
    {
        var entity = await Repository.GetAsync(input.Id);
        entity.IsActive = true;
        if (entity.UserId.HasValue)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId.Value.ToString());
            if (user != null) { user.IsActive = true; await _userManager.UpdateAsync(user); }
        }
        await CurrentUnitOfWork.SaveChangesAsync();
        return Ok<object>(null, L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input)
    {
        var entity = await Repository.GetAsync(input.Id);
        entity.IsActive = false;
        if (entity.UserId.HasValue)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId.Value.ToString());
            if (user != null) { user.IsActive = false; await _userManager.UpdateAsync(user); }
        }
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

    [HttpPost]
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

    [HttpPost]
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

    public async Task<ApiResponse<List<SalesmanSelectItemDto>>> GetSelectListAsync()
    {
        var list = await Repository.GetAll()
            .Where(s => s.IsActive)
            .Select(s => new SalesmanSelectItemDto
            {
                Id = s.Id,
                Name = s.Name,
                JobCode = s.JobCode
            })
            .ToListAsync();
        return Ok(list, L("RetrievedSuccessfully"));
    }

    private async Task SyncWarehousesAsync(int salesmanId, List<int> warehouseIds)
    {
        var existing = await _salesmanWarehouseRepository.GetAll()
            .Where(sw => sw.SalesmanId == salesmanId)
            .ToListAsync();

        foreach (var sw in existing)
            await _salesmanWarehouseRepository.DeleteAsync(sw);

        if (warehouseIds == null || warehouseIds.Count == 0)
            return;

        foreach (var wId in warehouseIds.Distinct())
        {
            await _salesmanWarehouseRepository.InsertAsync(new SalesmanWarehouse
            {
                SalesmanId = salesmanId,
                WarehouseId = wId
            });
        }

        await CurrentUnitOfWork.SaveChangesAsync();
    }
}
