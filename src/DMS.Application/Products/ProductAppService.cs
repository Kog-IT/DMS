using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using DMS.Products.Dto;
using Abp.Extensions;
using DMS.Authorization;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;

namespace DMS.Products
{
    public class ProductAppService : AsyncCrudAppService<
      Product,
      ProductDto,
      int,
      PagedProductResultRequestDto,
      CreateProductDto,
      UpdateProductDto>, IProductAppService
    {
        public ProductAppService(IRepository<Product, int> repository)
            : base(repository)
        {
           

            GetPermissionName = PermissionNames.Pages_Products;
            GetAllPermissionName = PermissionNames.Pages_Products;
            CreatePermissionName = PermissionNames.Pages_Products_Create;
            UpdatePermissionName = PermissionNames.Pages_Products_Edit;
            DeletePermissionName = PermissionNames.Pages_Products_Delete;

        }


        protected override IQueryable<Product> CreateFilteredQuery(PagedProductResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Category)
                .WhereIf(
                    !input.Keyword.IsNullOrWhiteSpace(), 
                    x => x.Name.Contains(input.Keyword) || x.Description.Contains(input.Keyword)
                )
                .WhereIf(
                    input.CategoryId.HasValue,
                    x => x.CategoryId == input.CategoryId.Value
                );
        }

        protected override ProductDto MapToEntityDto(Product entity)
        {
            var dto = base.MapToEntityDto(entity);
            

            if (entity.Category != null)
            {
                dto.CategoryName = entity.Category.Name;
            }

            return dto;
        }
        public override async Task<ProductDto> GetAsync(EntityDto<int> input)
        {
           
            var product = await Repository.GetAllIncluding(x => x.Category)
                                          .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (product == null)
            {
                throw new Abp.UI.UserFriendlyException("المنتج غير موجود!");
            }

            return MapToEntityDto(product);
        }

       


    }
}
