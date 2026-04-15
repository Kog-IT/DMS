using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DMS.Products.Dto
{
    public class ProductMapProfile : Profile
    {
        public ProductMapProfile()
        {
           
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            
        }
    }
}
