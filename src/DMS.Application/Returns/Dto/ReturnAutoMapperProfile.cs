using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DMS.Returns.Dto
{
    public class ReturnAutoMapperProfile : Profile
    {
        public ReturnAutoMapperProfile()
        {
            CreateMap<Return, ReturnDto>();
            CreateMap<ReturnLine, ReturnLineDto>();
           
            CreateMap<CreateReturnLineDto, ReturnLine>();
        }
    }
}
