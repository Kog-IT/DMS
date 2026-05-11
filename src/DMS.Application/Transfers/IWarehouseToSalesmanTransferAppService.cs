using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using System.Threading.Tasks;

namespace DMS.Transfers;

public interface IWarehouseToSalesmanTransferAppService
{
    Task<ApiResponse<WarehouseToSalesmanTransferDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<WarehouseToSalesmanTransferDto>>> GetAllAsync(PagedWarehouseToSalesmanTransferRequestDto input);
    Task<ApiResponse<WarehouseToSalesmanTransferDto>> CreateAsync(CreateWarehouseToSalesmanTransferDto input);
    Task<ApiResponse<WarehouseToSalesmanTransferDto>> UpdateAsync(UpdateWarehouseToSalesmanTransferDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input);
    Task<ApiResponse<object>> RejectAsync(RejectTransferDto input);
}
