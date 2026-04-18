using Abp.Domain.Repositories;
using Abp.Domain.Services;
using DMS.Customers;
using DMS.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.PriceLists;

public class PriceResolutionService : DomainService
{
    private readonly IRepository<PriceList, int> _priceListRepository;
    private readonly IRepository<PriceListItem, int> _itemRepository;
    private readonly IRepository<PriceListAssignment, int> _assignmentRepository;
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly IRepository<Product, int> _productRepository;

    public PriceResolutionService(
        IRepository<PriceList, int> priceListRepository,
        IRepository<PriceListItem, int> itemRepository,
        IRepository<PriceListAssignment, int> assignmentRepository,
        IRepository<Customer, int> customerRepository,
        IRepository<Product, int> productRepository)
    {
        _priceListRepository = priceListRepository;
        _itemRepository = itemRepository;
        _assignmentRepository = assignmentRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<PriceResolutionResult> ResolveAsync(int customerId, int productId, decimal quantity)
    {
        var today = DateTime.UtcNow;

        var customer = await _customerRepository.GetAsync(customerId);

        // 1. Customer-specific assignment
        var assignment = await _assignmentRepository.GetAll()
            .FirstOrDefaultAsync(a => a.CustomerId == customerId);

        PriceList list = null;

        if (assignment != null)
        {
            var assigned = await _priceListRepository.GetAll()
                .FirstOrDefaultAsync(p =>
                    p.Id == assignment.PriceListId &&
                    p.IsActive &&
                    p.StartDate <= today &&
                    (p.EndDate == null || p.EndDate >= today));
            list = assigned;
        }

        // 2. Classification-based list
        if (list == null && customer.Classification != CustomerClassification.Unclassified)
        {
            list = await _priceListRepository.GetAll()
                .Where(p =>
                    p.ForClassification == customer.Classification &&
                    p.IsActive &&
                    p.StartDate <= today &&
                    (p.EndDate == null || p.EndDate >= today))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefaultAsync();
        }

        // 3. Resolve price from list
        if (list != null)
        {
            var item = await _itemRepository.GetAll()
                .Where(i =>
                    i.PriceListId == list.Id &&
                    i.ProductId == productId &&
                    i.MinQuantity <= quantity)
                .OrderByDescending(i => i.MinQuantity)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                // Product in list but no tier covers quantity — use lowest tier
                item = await _itemRepository.GetAll()
                    .Where(i => i.PriceListId == list.Id && i.ProductId == productId)
                    .OrderBy(i => i.MinQuantity)
                    .FirstOrDefaultAsync();
            }

            if (item != null)
                return new PriceResolutionResult { Price = item.Price, IsBasePriceFallback = false, PriceListId = list.Id };
        }

        // 4. Fallback
        var product = await _productRepository.GetAsync(productId);
        return new PriceResolutionResult { Price = product.Price, IsBasePriceFallback = true, PriceListId = null };
    }
}
