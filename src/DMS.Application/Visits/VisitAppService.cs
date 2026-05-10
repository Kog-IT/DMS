using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Customers;
using DMS.Media;
using DMS.Visits.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Visits;

[AbpAuthorize(PermissionNames.Pages_Visits)]
public class VisitAppService : DmsCrudAppService<
    Visit,
    VisitDto,
    int,
    PagedVisitResultRequestDto,
    CreateVisitDto,
    UpdateVisitDto>, IVisitAppService
{
    private readonly IRepository<VisitPhoto, int> _photoRepository;
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly ISettingManager _settingManager;
    private readonly IRepository<MediaFile, int> _mediaRepository;

    public VisitAppService(
        IRepository<Visit, int> repository,
        IRepository<VisitPhoto, int> photoRepository,
        IRepository<Customer, int> customerRepository,
        ISettingManager settingManager,
        IRepository<MediaFile, int> mediaRepository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Visits;
        GetAllPermissionName = PermissionNames.Pages_Visits;
        CreatePermissionName = PermissionNames.Pages_Visits_Create;
        UpdatePermissionName = PermissionNames.Pages_Visits_Edit;
        DeletePermissionName = PermissionNames.Pages_Visits_Delete;

        _photoRepository = photoRepository;
        _customerRepository = customerRepository;
        _settingManager = settingManager;
        _mediaRepository = mediaRepository;
    }

    protected override VisitDto MapToEntityDto(Visit entity)
    {
        var dto = base.MapToEntityDto(entity);
        dto.Media = _mediaRepository.GetAll()
            .Where(m => m.MediaType == MediaType.Visit && m.ModelId == entity.Id)
            .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
            .ToList();
        return dto;
    }

    protected override async Task<Visit> GetEntityByIdAsync(int id)
        => await Repository.GetAll()
            .Include(v => v.Customer)
            .Include(v => v.Photos)
            .FirstOrDefaultAsync(v => v.Id == id)
            ?? throw new UserFriendlyException("Visit not found.");

    protected override IQueryable<Visit> CreateFilteredQuery(PagedVisitResultRequestDto input)
    {
        return Repository.GetAll()
            .Include(v => v.Customer)
            .WhereIf(input.Status.HasValue, v => v.Status == input.Status.Value)
            .WhereIf(input.AssignedUserId.HasValue, v => v.AssignedUserId == input.AssignedUserId.Value)
            .WhereIf(input.CustomerId.HasValue, v => v.CustomerId == input.CustomerId.Value)
            .WhereIf(input.DateFrom.HasValue, v => v.PlannedDate >= input.DateFrom.Value)
            .WhereIf(input.DateTo.HasValue, v => v.PlannedDate <= input.DateTo.Value);
    }

    [AbpAuthorize(PermissionNames.Pages_Visits_CheckIn)]
    public async Task<ApiResponse<VisitDto>> CheckInAsync(CheckInDto input)
    {
        var visit = await Repository.GetAsync(input.VisitId);

        if (visit.Status != VisitStatus.Planned)
            throw new UserFriendlyException("Only planned visits can be checked in.");

        var geofencingEnabled = await _settingManager.GetSettingValueAsync<bool>(VisitSettingNames.GeofencingEnabled);

        if (geofencingEnabled && input.Latitude.HasValue && input.Longitude.HasValue)
        {
            var customer = await _customerRepository.GetAsync(visit.CustomerId);
            if (customer.Latitude.HasValue && customer.Longitude.HasValue)
            {
                var distanceMeters = HaversineDistance(
                    input.Latitude.Value, input.Longitude.Value,
                    customer.Latitude.Value, customer.Longitude.Value);

                var radiusMeters = await _settingManager.GetSettingValueAsync<int>(VisitSettingNames.GeofencingRadiusMeters);
                var enforcement = await _settingManager.GetSettingValueAsync(VisitSettingNames.GpsEnforcement);

                if (distanceMeters > radiusMeters && enforcement == GpsEnforcementMode.Block)
                    throw new UserFriendlyException($"Check-in location is {distanceMeters:F0}m from customer. Required within {radiusMeters}m.");
            }
        }

        visit.Status = VisitStatus.InProgress;
        visit.CheckInTime = Clock.Now;
        visit.CheckInLatitude = input.Latitude;
        visit.CheckInLongitude = input.Longitude;

        await Repository.UpdateAsync(visit);
        return Ok(ObjectMapper.Map<VisitDto>(visit), L("UpdatedSuccessfully"));
    }

    [AbpAuthorize(PermissionNames.Pages_Visits_CheckOut)]
    public async Task<ApiResponse<VisitDto>> CheckOutAsync(CheckOutDto input)
    {
        var visit = await Repository.GetAsync(input.VisitId);

        if (visit.Status != VisitStatus.InProgress)
            throw new UserFriendlyException("Only in-progress visits can be checked out.");

        visit.Status = VisitStatus.Completed;
        visit.CheckOutTime = Clock.Now;
        visit.CheckOutLatitude = input.Latitude;
        visit.CheckOutLongitude = input.Longitude;
        visit.Notes = input.Notes;
        visit.NoSaleReason = input.NoSaleReason;

        if (visit.CheckInTime.HasValue)
            visit.DurationMinutes = (int)(visit.CheckOutTime.Value - visit.CheckInTime.Value).TotalMinutes;

        await Repository.UpdateAsync(visit);
        return Ok(ObjectMapper.Map<VisitDto>(visit), L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<VisitDto>> SkipAsync(SkipVisitDto input)
    {
        if (string.IsNullOrWhiteSpace(input.SkipReason))
            throw new UserFriendlyException("Skip reason is required.");

        var visit = await Repository.GetAsync(input.VisitId);

        if (visit.Status != VisitStatus.Planned && visit.Status != VisitStatus.InProgress)
            throw new UserFriendlyException("Only planned or in-progress visits can be skipped.");

        visit.Status = VisitStatus.Skipped;
        visit.SkipReason = input.SkipReason;

        await Repository.UpdateAsync(visit);
        return Ok(ObjectMapper.Map<VisitDto>(visit), L("UpdatedSuccessfully"));
    }

    public async Task<ApiResponse<VisitPhotoDto>> UploadPhotoAsync(UploadVisitPhotoDto input)
    {
        var allowedExtensions = new[] { "jpg", "jpeg", "png" };
        var ext = input.FileExtension?.ToLowerInvariant().TrimStart('.');
        if (!allowedExtensions.Contains(ext))
            throw new UserFriendlyException("Only jpg, jpeg, and png files are allowed.");

        var tenantId = AbpSession.TenantId ?? 0;
        var folder = Path.Combine("wwwroot", "uploads", "visits", tenantId.ToString(), input.VisitId.ToString());
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}.{ext}";
        var filePath = Path.Combine(folder, fileName);
        var bytes = Convert.FromBase64String(input.FileBase64);
        await File.WriteAllBytesAsync(filePath, bytes);

        var relativePath = Path.Combine("uploads", "visits", tenantId.ToString(), input.VisitId.ToString(), fileName)
            .Replace('\\', '/');

        var photo = new VisitPhoto
        {
            TenantId = tenantId,
            VisitId = input.VisitId,
            FilePath = relativePath,
            CapturedAt = input.CapturedAt,
            Caption = input.Caption
        };

        var savedPhoto = await _photoRepository.InsertAsync(photo);
        return Ok(ObjectMapper.Map<VisitPhotoDto>(savedPhoto), L("CreatedSuccessfully"));
    }

    public async Task<ApiResponse<List<SyncVisitResultDto>>> SyncVisitsAsync(List<SyncVisitDto> input)
    {
        var results = new List<SyncVisitResultDto>();

        foreach (var dto in input)
        {
            try
            {
                var existing = await Repository.GetAll()
                    .FirstOrDefaultAsync(v => v.ExternalId == dto.ExternalId && v.ExternalId != null);

                if (existing != null)
                {
                    results.Add(new SyncVisitResultDto { ExternalId = dto.ExternalId, Success = true, VisitId = existing.Id });
                    continue;
                }

                var visit = new Visit
                {
                    TenantId = AbpSession.TenantId ?? 0,
                    ExternalId = dto.ExternalId,
                    CustomerId = dto.CustomerId,
                    AssignedUserId = dto.AssignedUserId,
                    RouteId = dto.RouteId,
                    Status = dto.Status,
                    PlannedDate = dto.PlannedDate,
                    CheckInTime = dto.CheckInTime,
                    CheckOutTime = dto.CheckOutTime,
                    CheckInLatitude = dto.CheckInLatitude,
                    CheckInLongitude = dto.CheckInLongitude,
                    CheckOutLatitude = dto.CheckOutLatitude,
                    CheckOutLongitude = dto.CheckOutLongitude,
                    DurationMinutes = dto.DurationMinutes,
                    Notes = dto.Notes,
                    SkipReason = dto.SkipReason,
                    NoSaleReason = dto.NoSaleReason
                };

                var savedVisit = await Repository.InsertAsync(visit);

                foreach (var photoDto in dto.Photos)
                {
                    await UploadPhotoAsync(new UploadVisitPhotoDto
                    {
                        VisitId = savedVisit.Id,
                        FileBase64 = photoDto.FileBase64,
                        FileExtension = photoDto.FileExtension,
                        CapturedAt = photoDto.CapturedAt,
                        Caption = photoDto.Caption
                    });
                }

                results.Add(new SyncVisitResultDto { ExternalId = dto.ExternalId, Success = true, VisitId = savedVisit.Id });
            }
            catch (Exception ex)
            {
                results.Add(new SyncVisitResultDto { ExternalId = dto.ExternalId, Success = false, Error = ex.Message });
            }
        }

        return Ok(results, L("RetrievedSuccessfully"));
    }

    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
