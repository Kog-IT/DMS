using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using DMS.Authorization;
using DMS.Common;
using DMS.Common.Dto;
using DMS.Products.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DMS.Products
{
    public class ProductAppService : DmsCrudAppService<
      Product,
      ProductDto,
      int,
      PagedProductResultRequestDto,
      CreateProductDto,
      UpdateProductDto>, IProductAppService
    {
        private readonly IRepository<ProductVariant, int> _variantRepository;
        public ProductAppService(IRepository<Product, int> repository,
            IRepository<ProductVariant, int> variantRepository)
            : base(repository)
        {
            _variantRepository = variantRepository;

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

        public override async Task<ApiResponse<ProductDto>> GetAsync(EntityDto<int> input)
        {
            var product = await Repository.GetAllIncluding(x => x.Category)
                                          .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (product == null)
            {
                throw new UserFriendlyException("المنتج غير موجود!");
            }

            var dto = MapToEntityDto(product);
            return Ok(dto, L("RetrievedSuccessfully"));
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
        public async Task<ApiResponse<string>> UploadProductImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new UserFriendlyException("Only image files are allowed!");

            var folderName = Path.Combine("wwwroot", "images", "oils");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            var fileName = Guid.NewGuid().ToString() + extension;
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = "/images/oils/" + fileName;
            return Ok(result, L("CreatedSuccessfully"));
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
        public async Task<ApiResponse<ProductVariantDto>> CreateVariantAsync(CreateProductVariantDto input)
        {
            var product = await Repository.GetAsync(input.ProductId);

            var variant = ObjectMapper.Map<ProductVariant>(input);

            if (string.IsNullOrEmpty(variant.SKU))
            {
                var prefix = product.Name.Length >= 3 ? product.Name.Substring(0, 3).ToUpper() : product.Name.ToUpper();
                variant.SKU = $"{prefix}-{input.Capacity.Replace(" ", "")}";
            }

            var inserted = await _variantRepository.InsertAsync(variant);

            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = ObjectMapper.Map<ProductVariantDto>(inserted);
            return Ok(dto, L("CreatedSuccessfully"));
        }

        [AbpAuthorize(PermissionNames.Pages_Products)]
        public async Task<ApiResponse<List<ProductVariantDto>>> GetVariantsByProductIdAsync(int productId)
        {
            var variants = await _variantRepository.GetAllListAsync(v => v.ProductId == productId);

            var dto = ObjectMapper.Map<List<ProductVariantDto>>(variants);
            return Ok(dto, L("RetrievedSuccessfully"));
        }
    }
}
