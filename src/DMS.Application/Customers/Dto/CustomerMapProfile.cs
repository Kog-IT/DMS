using AutoMapper;
using DMS.Cities.Dto;
using DMS.Cities;

namespace DMS.Customers.Dto;

public class CustomerMapProfile : Profile
{
    public CustomerMapProfile()
    {
        CreateMap<City, CityDto>()
            .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.Governorate != null ? src.Governorate.Name : null));

        CreateMap<CreateCityDto, City>();
    }
}
