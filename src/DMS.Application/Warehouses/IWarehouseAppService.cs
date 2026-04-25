// src/DMS.Application/Warehouses/IWarehouseAppService.cs
using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.Warehouses.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Warehouses;

public interface IWarehouseAppService
{
    Task<ApiResponse<WarehouseDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<WarehouseDto>>> GetAllAsync(PagedWarehouseResultRequestDto input);
    Task<ApiResponse<WarehouseDto>> CreateAsync(CreateWarehouseDto input);
    Task<ApiResponse<WarehouseDto>> UpdateAsync(UpdateWarehouseDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);
    Task<ApiResponse<object>> ActivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> DeactivateAsync(EntityDto<int> input);
    Task<ApiResponse<object>> BulkDeleteAsync(List<int> ids);
    Task<ApiResponse<object>> BulkActivateAsync(List<int> ids);
    Task<ApiResponse<object>> BulkDeactivateAsync(List<int> ids);
    Task<ApiResponse<WarehouseWithProductsDto>> GetWarehouseWithProductsAsync(int id);
    Task<ApiResponse<object>> AssignProductsAsync(WarehouseProductCreateDto input);
}
