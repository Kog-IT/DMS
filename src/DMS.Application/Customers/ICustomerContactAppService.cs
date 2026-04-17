using Abp.Application.Services;
using DMS.Customers.Dto;

namespace DMS.Customers;

public interface ICustomerContactAppService : IAsyncCrudAppService<
    CustomerContactDto,
    int,
    PagedCustomerContactResultRequestDto,
    CreateCustomerContactDto,
    UpdateCustomerContactDto>
{
}
