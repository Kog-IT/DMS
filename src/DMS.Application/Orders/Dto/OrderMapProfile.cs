using AutoMapper;
using DMS.Orders.Dto;

namespace DMS.Orders.Dto;

public class OrderMapProfile : Profile
{
    public OrderMapProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Lines, opt => opt.MapFrom(s => s.Lines));

        CreateMap<OrderLine, OrderLineDto>();

        CreateMap<CreateOrderDto, Order>()
            .ForMember(d => d.Lines, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.SubTotal, opt => opt.Ignore())
            .ForMember(d => d.TaxTotal, opt => opt.Ignore())
            .ForMember(d => d.OrderDiscountAmount, opt => opt.Ignore())
            .ForMember(d => d.Total, opt => opt.Ignore())
            .ForMember(d => d.AssignedUserId, opt => opt.Ignore());

        CreateMap<UpdateOrderDto, Order>()
            .ForMember(d => d.Lines, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.SubTotal, opt => opt.Ignore())
            .ForMember(d => d.TaxTotal, opt => opt.Ignore())
            .ForMember(d => d.OrderDiscountAmount, opt => opt.Ignore())
            .ForMember(d => d.Total, opt => opt.Ignore())
            .ForMember(d => d.AssignedUserId, opt => opt.Ignore());
    }
}
