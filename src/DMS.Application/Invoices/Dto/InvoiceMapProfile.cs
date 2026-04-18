using AutoMapper;
using DMS.Invoices.Dto;

namespace DMS.Invoices;

public class InvoiceMapProfile : Profile
{
    public InvoiceMapProfile()
    {
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(d => d.Lines, opt => opt.MapFrom(s => s.Lines));

        CreateMap<InvoiceLine, InvoiceLineDto>();

        CreateMap<GenerateInvoiceDto, Invoice>()
            .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.OrderId))
            .ForMember(d => d.InvoiceNumber, opt => opt.Ignore())
            .ForMember(d => d.InvoiceDate, opt => opt.Ignore())
            .ForMember(d => d.DueDate, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.CustomerName, opt => opt.Ignore())
            .ForMember(d => d.CustomerAddress, opt => opt.Ignore())
            .ForMember(d => d.SubTotal, opt => opt.Ignore())
            .ForMember(d => d.TaxTotal, opt => opt.Ignore())
            .ForMember(d => d.DiscountAmount, opt => opt.Ignore())
            .ForMember(d => d.Total, opt => opt.Ignore())
            .ForMember(d => d.PaidAmount, opt => opt.Ignore())
            .ForMember(d => d.Notes, opt => opt.Ignore())
            .ForMember(d => d.VoidReason, opt => opt.Ignore())
            .ForMember(d => d.Lines, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore());
    }
}
