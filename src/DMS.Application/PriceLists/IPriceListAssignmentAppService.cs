using Abp.Application.Services;
using DMS.PriceLists.Dto;
using System.Threading.Tasks;

namespace DMS.PriceLists;

public interface IPriceListAssignmentAppService : IApplicationService
{
    Task AssignToCustomerAsync(AssignPriceListDto input);
    Task RemoveAssignmentAsync(int customerId);
    Task<PriceListAssignmentDto> GetAssignmentAsync(int customerId);
}
