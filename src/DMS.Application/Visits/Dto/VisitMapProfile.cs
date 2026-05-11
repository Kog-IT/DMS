using AutoMapper;

namespace DMS.Visits.Dto;

public class VisitMapProfile : Profile
{
    public VisitMapProfile()
    {
        CreateMap<Visit, VisitDto>()
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer != null ? s.Customer.Name : null))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Photos, opt => opt.MapFrom(s => s.Photos));
    }
}
