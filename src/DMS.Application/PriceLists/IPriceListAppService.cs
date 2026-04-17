using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DMS.PriceLists.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.PriceLists;

public interface IPriceListAppService : IAsyncCrudAppService<
    PriceListDto,
    int,
    PagedPriceListRequestDto,
    CreatePriceListDto,
    UpdatePriceListDto>
{
    Task<List<PriceListItemDto>> GetItemsAsync(int priceListId);
    Task SetItemsAsync(SetPriceListItemsDto input);
}
