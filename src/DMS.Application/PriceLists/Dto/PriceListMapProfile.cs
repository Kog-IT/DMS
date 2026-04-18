using AutoMapper;

namespace DMS.PriceLists.Dto;

public class PriceListMapProfile : Profile
{
    public PriceListMapProfile()
    {
        CreateMap<PriceList, PriceListDto>();
        CreateMap<CreatePriceListDto, PriceList>();
        CreateMap<UpdatePriceListDto, PriceList>();

        CreateMap<PriceListItem, PriceListItemDto>();

        CreateMap<PriceListAssignment, PriceListAssignmentDto>()
            .ForMember(d => d.PriceListName,
                opt => opt.MapFrom(src => src.PriceList != null ? src.PriceList.Name : string.Empty));
    }
}
