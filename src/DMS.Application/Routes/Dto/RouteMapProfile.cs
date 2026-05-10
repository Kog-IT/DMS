using AutoMapper;

namespace DMS.Routes.Dto;

public class RouteMapProfile : Profile
{
    public RouteMapProfile()
    {
        CreateMap<Route, RouteDto>()
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<RouteItem, RouteItemDto>()
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer != null ? s.Customer.Name : null))
            .ForMember(d => d.VisitId, opt => opt.MapFrom(s => s.Visit != null ? (int?)s.Visit.Id : null));

        CreateMap<UpdateRouteItemDto, RouteItem>()
            .ForMember(d => d.RouteId, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Customer, opt => opt.Ignore())
            .ForMember(d => d.Visit, opt => opt.Ignore())
            .ForMember(d => d.Route, opt => opt.Ignore());
    }
}
