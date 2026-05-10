using Abp.Application.Services.Dto;
using DMS.Common.Dto;
using DMS.PriceLists.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.PriceLists;

public interface IPriceListAppService
{
    Task<ApiResponse<PriceListDto>> GetAsync(EntityDto<int> input);
    Task<ApiResponse<PagedResultDto<PriceListDto>>> GetAllAsync(PagedPriceListRequestDto input);
    Task<ApiResponse<PriceListDto>> CreateAsync(CreatePriceListDto input);
    Task<ApiResponse<PriceListDto>> UpdateAsync(UpdatePriceListDto input);
    Task<ApiResponse<object>> DeleteAsync(EntityDto<int> input);

    Task<ApiResponse<List<PriceListItemDto>>> GetItemsAsync(int priceListId);
    Task<ApiResponse<object>> SetItemsAsync(SetPriceListItemsDto input);
}
