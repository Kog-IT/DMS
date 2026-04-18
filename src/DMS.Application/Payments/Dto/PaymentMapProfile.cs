using AutoMapper;

namespace DMS.Payments.Dto;

public class PaymentMapProfile : Profile
{
    public PaymentMapProfile()
    {
        CreateMap<PaymentMethod, PaymentMethodDto>();
        CreateMap<CreatePaymentMethodDto, PaymentMethod>();
        CreateMap<UpdatePaymentMethodDto, PaymentMethod>();

        CreateMap<Payment, PaymentDto>();
        CreateMap<PaymentLine, PaymentLineDto>()
            .ForMember(d => d.PaymentMethodName,
                opt => opt.MapFrom(src => src.PaymentMethod != null ? src.PaymentMethod.Name : string.Empty));
    }
}
