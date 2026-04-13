using AutoMapper;
using DMS.Companies;

namespace DMS.Companies.Dto;

public class CompanyMapProfile : Profile
{
    public CompanyMapProfile()
    {
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();
    }
}
