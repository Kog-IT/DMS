using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Orders.Dto;

namespace DMS.Orders;

public interface IOrderAppService : IAsyncCrudAppService<
    OrderDto,
    int,
    PagedOrderResultRequestDto,
    CreateOrderDto,
    UpdateOrderDto>
{
    Task SubmitAsync(int id);
    Task ApproveAsync(int id);
    Task RejectAsync(int id, string reason);
    Task CancelAsync(int id);
    Task MarkDeliveredAsync(int id);
}
