using AutoMapper;

namespace DMS.Governorates.Dto
{
    public class GovernorateMapProfile : Profile
    {
        public GovernorateMapProfile()
        {
            CreateMap<Governorate, GovernorateDto>()
                .ForMember(d => d.Cities, opt => opt.MapFrom(s => s.Cities));

            CreateMap<CreateGovernorateDto, Governorate>();
            CreateMap<UpdateGovernorateDto, Governorate>();
        }
    }
}
