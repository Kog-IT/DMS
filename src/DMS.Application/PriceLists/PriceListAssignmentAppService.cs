using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using DMS.Authorization;
using DMS.PriceLists.Dto;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DMS.PriceLists;

[AbpAuthorize(PermissionNames.Pages_PriceLists_Assign)]
public class PriceListAssignmentAppService : DMSAppServiceBase, IPriceListAssignmentAppService
{
    private readonly IRepository<PriceListAssignment, int> _assignmentRepository;
    private readonly IRepository<PriceList, int> _priceListRepository;

    public PriceListAssignmentAppService(
        IRepository<PriceListAssignment, int> assignmentRepository,
        IRepository<PriceList, int> priceListRepository)
    {
        _assignmentRepository = assignmentRepository;
        _priceListRepository = priceListRepository;
    }

    public async Task AssignToCustomerAsync(AssignPriceListDto input)
    {
        var listExists = await _priceListRepository.GetAll()
            .AnyAsync(p => p.Id == input.PriceListId);
        if (!listExists)
            throw new UserFriendlyException("Price list not found.");

        var existing = await _assignmentRepository.GetAll()
            .FirstOrDefaultAsync(a => a.CustomerId == input.CustomerId);

        if (existing != null)
        {
            existing.PriceListId = input.PriceListId;
            await _assignmentRepository.UpdateAsync(existing);
        }
        else
        {
            await _assignmentRepository.InsertAsync(new PriceListAssignment
            {
                TenantId = AbpSession.TenantId!.Value,
                CustomerId = input.CustomerId,
                PriceListId = input.PriceListId
            });
        }
    }

    public async Task RemoveAssignmentAsync(int customerId)
    {
        var existing = await _assignmentRepository.GetAll()
            .FirstOrDefaultAsync(a => a.CustomerId == customerId);

        if (existing != null)
            await _assignmentRepository.DeleteAsync(existing);
    }

    public async Task<PriceListAssignmentDto> GetAssignmentAsync(int customerId)
    {
        var assignment = await _assignmentRepository.GetAll()
            .Include(a => a.PriceList)
            .FirstOrDefaultAsync(a => a.CustomerId == customerId);

        return assignment == null ? null : ObjectMapper.Map<PriceListAssignmentDto>(assignment);
    }
}
