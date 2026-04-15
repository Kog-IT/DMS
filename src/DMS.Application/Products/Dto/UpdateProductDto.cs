using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DMS.Products.Dto
{
    [AutoMapTo(typeof(Product))]
    public class UpdateProductDto : CreateProductDto, IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
