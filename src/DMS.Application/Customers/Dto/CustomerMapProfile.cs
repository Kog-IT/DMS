using AutoMapper;
using DMS.Cities.Dto;
using DMS.Cities;
using DMS.Governorates.Dto;
using DMS.Governorates;

namespace DMS.Customers.Dto;

// Mappings handled via [AutoMapFrom]/[AutoMapTo] attributes on each DTO.
// This profile exists as an extension point for future ForMember configurations.
public class CustomerMapProfile : Profile
{
    public CustomerMapProfile()
    {
        CreateMap<City, CityDto>()
            .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.Governorate.Name));

        CreateMap<CreateCityDto, City>();
        CreateMap<CreateGovernorateDto, Governorate>();
    }
}
