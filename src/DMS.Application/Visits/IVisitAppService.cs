using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Visits.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Visits;

public interface IVisitAppService
{
    Task<ApiResponse<VisitDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<VisitDto>>> GetAllAsync(PagedVisitResultRequestDto input);
    Task<ApiResponse<VisitDto>> CreateAsync(CreateVisitDto input);
    Task<ApiResponse<VisitDto>> UpdateAsync(UpdateVisitDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<VisitDto>> CheckInAsync(CheckInDto input);
    Task<ApiResponse<VisitDto>> CheckOutAsync(CheckOutDto input);
    Task<ApiResponse<VisitDto>> SkipAsync(SkipVisitDto input);
    Task<ApiResponse<VisitPhotoDto>> UploadPhotoAsync(UploadVisitPhotoDto input);
    Task<ApiResponse<List<SyncVisitResultDto>>> SyncVisitsAsync(List<SyncVisitDto> input);
}
