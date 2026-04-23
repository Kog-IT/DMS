using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Orders.Dto;
using System.Threading.Tasks;

namespace DMS.Orders;

public interface IOrderAppService
{
    Task<ApiResponse<OrderDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<OrderDto>>> GetAllAsync(PagedOrderResultRequestDto input);
    Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto input);
    Task<ApiResponse<OrderDto>> UpdateAsync(UpdateOrderDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> SubmitAsync(int id);
    Task<ApiResponse<object>> ApproveAsync(int id);
    Task<ApiResponse<object>> RejectAsync(int id, string reason);
    Task<ApiResponse<object>> CancelAsync(int id);
    Task<ApiResponse<object>> MarkDeliveredAsync(int id);
}
