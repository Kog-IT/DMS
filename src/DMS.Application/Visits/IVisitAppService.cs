using Abp.Application.Services;
using DMS.Visits.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Visits;

public interface IVisitAppService : IAsyncCrudAppService<
    VisitDto,
    int,
    PagedVisitResultRequestDto,
    CreateVisitDto,
    UpdateVisitDto>
{
    Task<VisitDto> CheckInAsync(CheckInDto input);
    Task<VisitDto> CheckOutAsync(CheckOutDto input);
    Task<VisitDto> SkipAsync(SkipVisitDto input);
    Task<VisitPhotoDto> UploadPhotoAsync(UploadVisitPhotoDto input);
    Task<List<SyncVisitResultDto>> SyncVisitsAsync(List<SyncVisitDto> input);
}
