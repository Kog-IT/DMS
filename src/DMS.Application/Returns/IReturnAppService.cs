using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Returns.Dto;

namespace DMS.Returns
{
    public interface IReturnAppService : IApplicationService
    {
        Task<ReturnDto> CreateAsync(CreateReturnDto input);
    }
}
