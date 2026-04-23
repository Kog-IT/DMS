using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Customers.Dto;
using System.Threading.Tasks;

namespace DMS.Customers;

public interface ICustomerAppService
{
    Task<ApiResponse<CustomerDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<CustomerDto>>> GetAllAsync(PagedCustomerResultRequestDto input);
    Task<ApiResponse<CustomerDto>> CreateAsync(CreateCustomerDto input);
    Task<ApiResponse<CustomerDto>> UpdateAsync(UpdateCustomerDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);

    Task<ApiResponse<CreditStatusDto>> GetCreditStatusAsync(int customerId);
    Task<ApiResponse<object>> BlockAsync(int customerId, string reason);
    Task<ApiResponse<object>> UnblockAsync(int customerId);
    Task<ApiResponse<object>> UpdateCreditLimitAsync(int customerId, UpdateCreditLimitDto input);
}
