using Abp.Application.Services;
using DMS.Customers.Dto;
using System.Threading.Tasks;

namespace DMS.Customers;

public interface ICustomerAppService : IAsyncCrudAppService<
    CustomerDto,
    int,
    PagedCustomerResultRequestDto,
    CreateCustomerDto,
    UpdateCustomerDto>
{
    Task<CreditStatusDto> GetCreditStatusAsync(int customerId);
}
