using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Transfers.Dto;
using System.Threading.Tasks;

namespace DMS.Transfers;

public interface ISalesmanToSalesmanTransferAppService
{
    Task<ApiResponse<SalesmanToSalesmanTransferDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<SalesmanToSalesmanTransferDto>>> GetAllAsync(PagedSalesmanToSalesmanTransferRequestDto input);
    Task<ApiResponse<SalesmanToSalesmanTransferDto>> CreateAsync(CreateSalesmanToSalesmanTransferDto input);
    Task<ApiResponse<SalesmanToSalesmanTransferDto>> UpdateAsync(UpdateSalesmanToSalesmanTransferDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ApproveAsync(EntityDto<int> input);
    Task<ApiResponse<object>> RejectAsync(RejectTransferDto input);
}
