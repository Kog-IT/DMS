using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Governorates.Dto;

namespace DMS.Governorates
{
    public interface IGovernorateAppService :
        IAsyncCrudAppService<GovernorateDto, int, PagedGovernorateResultRequestDto, CreateGovernorateDto, UpdateGovernorateDto>
    {
    }
}
