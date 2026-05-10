using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using System.Threading.Tasks;

namespace DMS.Transfers;

public interface IWarehouseToWarehouseTransferAppService
{
    Task<ApiResponse<WarehouseToWarehouseTransferDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<WarehouseToWarehouseTransferDto>>> GetAllAsync(PagedWarehouseToWarehouseTransferRequestDto input);
    Task<ApiResponse<WarehouseToWarehouseTransferDto>> CreateAsync(CreateWarehouseToWarehouseTransferDto input);
    Task<ApiResponse<WarehouseToWarehouseTransferDto>> UpdateAsync(UpdateWarehouseToWarehouseTransferDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
