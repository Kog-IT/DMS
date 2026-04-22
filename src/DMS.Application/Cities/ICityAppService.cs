using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Cities.Dto;

namespace DMS.Cities
{
    public interface ICityAppService : IAsyncCrudAppService<CityDto, int, PagedCityResultRequestDto, CreateCityDto,UpdateCityDto>
    {
    }
}
