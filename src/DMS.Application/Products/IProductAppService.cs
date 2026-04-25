using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Products.Dto;

namespace DMS.Products
{
    public interface IProductAppService
    {
        Task<ApiResponse<ProductDto>> GetAsync(EntityDto<int> input);
        Task<ApiResponse<PagedResultDto<ProductDto>>> GetAllAsync(PagedProductResultRequestDto input);
        Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto input);
        Task<ApiResponse<ProductDto>> UpdateAsync(UpdateProductDto input);
        Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);

        Task<ApiResponse<string>> UploadProductImage(Microsoft.AspNetCore.Http.IFormFile file);
        Task<ApiResponse<ProductVariantDto>> CreateVariantAsync(CreateProductVariantDto input);
        Task<ApiResponse<List<ProductVariantDto>>> GetVariantsByProductIdAsync(int productId);
    }
}
