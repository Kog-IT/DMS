using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Customers.Dto;
using System.Threading.Tasks;

namespace DMS.Customers;

public interface ICustomerContactAppService
{
    Task<ApiResponse<CustomerContactDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<CustomerContactDto>>> GetAllAsync(PagedCustomerContactResultRequestDto input);
    Task<ApiResponse<CustomerContactDto>> CreateAsync(CreateCustomerContactDto input);
    Task<ApiResponse<CustomerContactDto>> UpdateAsync(UpdateCustomerContactDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
}
