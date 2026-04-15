using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using DMS.Products.Dto;

namespace DMS.Products
{
    public interface IProductAppService : IAsyncCrudAppService<
    ProductDto,
    int,
    PagedProductResultRequestDto,
    CreateProductDto,
    UpdateProductDto>
    {
    }
}
