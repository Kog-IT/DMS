using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using System.Threading.Tasks;

namespace DMS.Transfers;

public interface ISalesmanToWarehouseTransferAppService
{
    Task<ApiResponse<SalesmanToWarehouseTransferDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<SalesmanToWarehouseTransferDto>>> GetAllAsync(PagedSalesmanToWarehouseTransferRequestDto input);
    Task<ApiResponse<SalesmanToWarehouseTransferDto>> CreateAsync(CreateSalesmanToWarehouseTransferDto input);
    Task<ApiResponse<SalesmanToWarehouseTransferDto>> UpdateAsync(UpdateSalesmanToWarehouseTransferDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input);
    Task<ApiResponse<object>> RejectAsync(RejectTransferDto input);
}
