using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Customers.Dto;

namespace DMS.Customers;

public interface IClassifyCustomersAppService : IApplicationService
{
    Task<List<ClassificationResultDto>> ClassifyAllAsync();
}
