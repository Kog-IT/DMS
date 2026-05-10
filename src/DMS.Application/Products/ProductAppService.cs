using Microsoft.AspNetCore.Mvc;
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
        private readonly IRepository<Product, int> _productRepository;
         private readonly IRepository<DMS.Media.MediaFile, int> _mediaRepository;

        public ProductAppService(IRepository<Product, int> productRepository,
            IRepository<ProductVariant, int> variantRepository,
            IRepository<DMS.Media.MediaFile, int> mediaRepository)
            : base(productRepository)
        {
            _variantRepository = variantRepository;
            _productRepository = productRepository; 
            _mediaRepository = mediaRepository;

            GetPermissionName = PermissionNames.Pages_Products;
            GetAllPermissionName = PermissionNames.Pages_Products;
            CreatePermissionName = PermissionNames.Pages_Products_Create;
            UpdatePermissionName = PermissionNames.Pages_Products_Edit;
            DeletePermissionName = PermissionNames.Pages_Products_Delete;
        }

        public override async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
           
            if (!string.IsNullOrEmpty(input.Barcode))
            {
                var isBarcodeExists = await _productRepository.GetAll().AnyAsync(p => p.Barcode == input.Barcode);
                if (isBarcodeExists)
                {
                    throw new UserFriendlyException("عفواً، هذا الباركود مسجل مسبقاً لمنتج آخر!");
                }
            }

          
            var isSkuExists = await _productRepository.GetAll().AnyAsync(p => p.SKU == input.SKU);
            if (isSkuExists)
            {
                throw new UserFriendlyException("عفواً، هذا الـ SKU مستخدم بالفعل!");
            }

            return await base.CreateAsync(input);
        }

        public override async Task<ProductDto> UpdateAsync(UpdateProductDto input)
        {
            
            if (!string.IsNullOrEmpty(input.Barcode))
            {
                var isBarcodeExists = await _productRepository.GetAll().AnyAsync(p =>
                    p.Barcode == input.Barcode); 

                if (isBarcodeExists)
                {
                    throw new UserFriendlyException("هذا الباركود مستخدم بالفعل في منتج آخر!");
                }
            }

            return await base.UpdateAsync(input);
        }

        protected override IQueryable<Product> CreateFilteredQuery(PagedProductResultRequestDto input)
        { 
        
            return Repository.GetAll()
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.ProductGroup)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Name.Contains(input.Keyword) || x.Description.Contains(input.Keyword))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(),
                    x => x.Code.Contains(input.Code))
                .WhereIf(input.CategoryId.HasValue,
                    x => x.CategoryId == input.CategoryId.Value)
                .WhereIf(input.ProductGroupId.HasValue,
                    x => x.ProductGroupId == input.ProductGroupId.Value)
                .WhereIf(input.BrandId.HasValue,
                    x => x.BrandId == input.BrandId.Value)
                .WhereIf(input.ProductStatus.HasValue,
                    x => x.ProductStatus == input.ProductStatus.Value)
                .WhereIf(input.Grade.HasValue,
                    x => x.Grade == input.Grade.Value)
                .WhereIf(input.Unit.HasValue,
                    x => x.Unit == input.Unit.Value)
                .WhereIf(input.IsActive.HasValue,
                    x => x.IsActive == input.IsActive.Value);
        }

        protected override ProductDto MapToEntityDto(Product entity)
        {
            var dto = base.MapToEntityDto(entity);

            if (entity.Category != null)
                dto.CategoryName = entity.Category.Name;

            if (entity.Brand != null)
                dto.BrandName = entity.Brand.Name;

            if (entity.ProductGroup != null)
                dto.ProductGroupName = entity.ProductGroup.Name;

            dto.Media = _mediaRepository.GetAll()
                .Where(m => m.MediaType == DMS.Media.MediaType.Product && m.ModelId == entity.Id)
                .Select(m => new DMS.Application.Media.Dto.MediaItemDto { Id = m.Id, Path = m.FilePath })
                .ToList();

            return dto;
        }

        public override async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto input)
        {
            var result = await base.CreateAsync(input);
            var productId = result.Data.Id;
            await SaveMediaPathsAsync(productId, input.Paths, replace: false);
            return await GetAsync(new EntityDto<int>(productId));
        }

        public override async Task<ApiResponse<ProductDto>> UpdateAsync(UpdateProductDto input)
        {
            await base.UpdateAsync(input);
            await SaveMediaPathsAsync(input.Id, input.Paths, replace: true);
            return await GetAsync(new EntityDto<int>(input.Id));
        }

        private async Task SaveMediaPathsAsync(int productId, List<string> paths, bool replace)
        {
            if (paths == null || paths.Count == 0) return;
            var tenantId = AbpSession.TenantId ?? 0;

            if (replace)
            {
                var existing = _mediaRepository.GetAll()
                    .Where(m => m.MediaType == DMS.Media.MediaType.Product && m.ModelId == productId)
                    .ToList();
                foreach (var m in existing)
                    await _mediaRepository.DeleteAsync(m);
            }

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                await _mediaRepository.InsertAsync(new DMS.Media.MediaFile
                {
                    TenantId = tenantId,
                    MediaType = DMS.Media.MediaType.Product,
                    ModelId = productId,
                    FilePath = path,
                    FileName = Path.GetFileName(path),
                    ContentType = GetContentType(path)
                });
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }

        public override async Task<ApiResponse<ProductDto>> GetAsync(EntityDto<int> input)
        {
            var product = await Repository.GetAll()
                                          .Include(x => x.Category)
                                          .Include(x => x.Brand)
                                          .Include(x => x.ProductGroup)
                                          .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (product == null)
            {
                throw new UserFriendlyException("المنتج غير موجود!");
            }

            var dto = MapToEntityDto(product);
            return Ok(dto, L("RetrievedSuccessfully"));
        }

        public async Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        public async Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        [HttpPost]
        public async Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("DeletedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                await Repository.DeleteAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("DeletedSuccessfully"));
        }

        [HttpPost]
        public async Task<ApiResponse<object>> BulkActivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        [HttpPost]
        public async Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Ok<object>(null, L("UpdatedSuccessfully"));
            var entities = await Repository.GetAll()
                .Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in entities)
                entity.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return Ok<object>(null, L("UpdatedSuccessfully"));
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
        [Microsoft.AspNetCore.Mvc.Consumes("multipart/form-data")]
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

        public async Task<ProductDto> GetByBarcodeAsync(GetProductByBarcodeDto input)
        {
            
            var product = await Repository.GetAllIncluding(p => p.Category, p => p.Variants)
                .FirstOrDefaultAsync(p => p.Barcode == input.Barcode);

            if (product == null)
            {
                throw new UserFriendlyException("عفواً، لا يوجد منتج مسجل بهذا الباركود!");
            }

            return ObjectMapper.Map<ProductDto>(product);
        }
    }
}
