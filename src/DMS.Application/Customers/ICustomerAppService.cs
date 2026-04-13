using Abp.Application.Services;
using DMS.Customers.Dto;

namespace DMS.Customers;

public interface ICustomerAppService : IAsyncCrudAppService<
    CustomerDto,
    int,
    PagedCustomerResultRequestDto,
    CreateCustomerDto,
    UpdateCustomerDto>
{
}
