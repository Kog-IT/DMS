using Abp.Application.Services.Dto;
using Abp.UI;
using DMS.Categories;
using DMS.Customers;
using DMS.Orders;
using DMS.Orders.Dto;
using DMS.PriceLists;
using DMS.PriceLists.Dto;
using DMS.Products;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.PriceLists;

public class PriceList_Tests : DMSTestBase
{
    private readonly IPriceListAppService _priceListService;
    private readonly IPriceListAssignmentAppService _assignmentService;
    private readonly IOrderAppService _orderService;

    public PriceList_Tests()
    {
        _priceListService = Resolve<IPriceListAppService>();
        _assignmentService = Resolve<IPriceListAssignmentAppService>();
        _orderService = Resolve<IOrderAppService>();
    }

    private async Task<int> SeedCustomerAsync(string code, CustomerClassification classification = CustomerClassification.Unclassified)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var c = new Customer { TenantId = 1, Code = code, Name = "Test", IsActive = true, Classification = classification };
            ctx.Set<Customer>().Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        });
        return id;
    }

    private async Task<int> SeedProductAsync(string name, decimal price, decimal taxRate = 0)
    {
        int id = 0;
        await UsingDbContextAsync(async ctx =>
        {
            var cat = new Category { TenantId = 1, Name = $"Cat_{Guid.NewGuid():N}" };
            ctx.Set<Category>().Add(cat);
            await ctx.SaveChangesAsync();
            var p = new Product { TenantId = 1, Name = name, Price = price, TaxRate = taxRate, CategoryId = cat.Id };
            ctx.Set<Product>().Add(p);
            await ctx.SaveChangesAsync();
            id = p.Id;
        });
        return id;
    }

    private async Task<int> CreatePriceListAsync(
        string name,
        CustomerClassification? forClassification = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var dto = await _priceListService.CreateAsync(new CreatePriceListDto
        {
            Name = name,
            StartDate = startDate ?? DateTime.UtcNow.AddDays(-1),
            EndDate = endDate,
            IsActive = true,
            ForClassification = forClassification
        });
        return dto.Id;
    }

    [Fact]
    public async Task Order_Uses_BasePriceFallback_When_No_PriceList()
    {
        var custId = await SeedCustomerAsync("C001");
        var prodId = await SeedProductAsync("Cola", price: 10m);

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = custId,
            OrderDate = DateTime.UtcNow,
            Lines = new List<CreateOrderLineDto>
            {
                new() { ProductId = prodId, Quantity = 1, DiscountType = DiscountType.None, DiscountValue = 0 }
            }
        });

        order.Lines[0].UnitPrice.ShouldBe(10m);
        order.Lines[0].IsBasePriceFallback.ShouldBeTrue();
    }

    [Fact]
    public async Task Classification_List_Overrides_Base_Price()
    {
        var custId = await SeedCustomerAsync("C002", CustomerClassification.A);
        var prodId = await SeedProductAsync("Cola2", price: 10m);

        var listId = await CreatePriceListAsync("Class A List", forClassification: CustomerClassification.A);
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = listId,
            Items = new List<PriceListItemInputDto>
            {
                new() { ProductId = prodId, MinQuantity = 1, Price = 8m }
            }
        });

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = custId,
            OrderDate = DateTime.UtcNow,
            Lines = new List<CreateOrderLineDto>
            {
                new() { ProductId = prodId, Quantity = 1, DiscountType = DiscountType.None, DiscountValue = 0 }
            }
        });

        order.Lines[0].UnitPrice.ShouldBe(8m);
        order.Lines[0].IsBasePriceFallback.ShouldBeFalse();
    }

    [Fact]
    public async Task Customer_Specific_List_Wins_Over_Classification_List()
    {
        var custId = await SeedCustomerAsync("C003", CustomerClassification.A);
        var prodId = await SeedProductAsync("Cola3", price: 10m);

        var classListId = await CreatePriceListAsync("Class A List2", forClassification: CustomerClassification.A);
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = classListId,
            Items = new List<PriceListItemInputDto> { new() { ProductId = prodId, MinQuantity = 1, Price = 8m } }
        });

        var custListId = await CreatePriceListAsync("VIP List");
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = custListId,
            Items = new List<PriceListItemInputDto> { new() { ProductId = prodId, MinQuantity = 1, Price = 6m } }
        });
        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = custListId });

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = custId,
            OrderDate = DateTime.UtcNow,
            Lines = new List<CreateOrderLineDto>
            {
                new() { ProductId = prodId, Quantity = 1, DiscountType = DiscountType.None, DiscountValue = 0 }
            }
        });

        order.Lines[0].UnitPrice.ShouldBe(6m);
        order.Lines[0].IsBasePriceFallback.ShouldBeFalse();
    }

    [Fact]
    public async Task Expired_Customer_List_Falls_Through_To_Classification_List()
    {
        var custId = await SeedCustomerAsync("C004", CustomerClassification.B);
        var prodId = await SeedProductAsync("Cola4", price: 10m);

        var expiredListId = await CreatePriceListAsync("Expired VIP",
            startDate: DateTime.UtcNow.AddDays(-30),
            endDate: DateTime.UtcNow.AddDays(-1));
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = expiredListId,
            Items = new List<PriceListItemInputDto> { new() { ProductId = prodId, MinQuantity = 1, Price = 5m } }
        });
        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = expiredListId });

        var classListId = await CreatePriceListAsync("Class B List", forClassification: CustomerClassification.B);
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = classListId,
            Items = new List<PriceListItemInputDto> { new() { ProductId = prodId, MinQuantity = 1, Price = 7m } }
        });

        var order = await _orderService.CreateAsync(new CreateOrderDto
        {
            CustomerId = custId,
            OrderDate = DateTime.UtcNow,
            Lines = new List<CreateOrderLineDto>
            {
                new() { ProductId = prodId, Quantity = 1, DiscountType = DiscountType.None, DiscountValue = 0 }
            }
        });

        order.Lines[0].UnitPrice.ShouldBe(7m);
    }

    [Fact]
    public async Task Correct_Tier_Selected_For_Quantity()
    {
        var custId = await SeedCustomerAsync("C005", CustomerClassification.A);
        var prodId = await SeedProductAsync("Cola5", price: 10m);

        var listId = await CreatePriceListAsync("Tiered", forClassification: CustomerClassification.A);
        await _priceListService.SetItemsAsync(new SetPriceListItemsDto
        {
            PriceListId = listId,
            Items = new List<PriceListItemInputDto>
            {
                new() { ProductId = prodId, MinQuantity = 1,  Price = 10m },
                new() { ProductId = prodId, MinQuantity = 10, Price = 9m  },
                new() { ProductId = prodId, MinQuantity = 50, Price = 8m  }
            }
        });

        async Task<decimal> GetPrice(int qty)
        {
            var o = await _orderService.CreateAsync(new CreateOrderDto
            {
                CustomerId = custId,
                OrderDate = DateTime.UtcNow,
                Lines = new List<CreateOrderLineDto>
                {
                    new() { ProductId = prodId, Quantity = qty, DiscountType = DiscountType.None, DiscountValue = 0 }
                }
            });
            return o.Lines[0].UnitPrice;
        }

        (await GetPrice(1)).ShouldBe(10m);
        (await GetPrice(10)).ShouldBe(9m);
        (await GetPrice(50)).ShouldBe(8m);
        (await GetPrice(5)).ShouldBe(10m);
    }

    [Fact]
    public async Task Delete_List_With_Assignments_Is_Blocked()
    {
        var custId = await SeedCustomerAsync("C006");
        var listId = await CreatePriceListAsync("Protected List");
        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = listId });

        await Should.ThrowAsync<UserFriendlyException>(
            () => _priceListService.DeleteAsync(new EntityDto<int>(listId)));
    }

    [Fact]
    public async Task EndDate_Before_StartDate_Is_Rejected()
    {
        await Should.ThrowAsync<UserFriendlyException>(() =>
            _priceListService.CreateAsync(new CreatePriceListDto
            {
                Name = "Bad Dates",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            }));
    }

    [Fact]
    public async Task SetItems_Duplicate_ProductId_MinQuantity_Is_Rejected()
    {
        var prodId = await SeedProductAsync("ColaX", 10m);
        var listId = await CreatePriceListAsync("Dup Test");

        await Should.ThrowAsync<UserFriendlyException>(() =>
            _priceListService.SetItemsAsync(new SetPriceListItemsDto
            {
                PriceListId = listId,
                Items = new List<PriceListItemInputDto>
                {
                    new() { ProductId = prodId, MinQuantity = 1, Price = 8m },
                    new() { ProductId = prodId, MinQuantity = 1, Price = 7m }
                }
            }));
    }

    [Fact]
    public async Task AssignToCustomer_Upserts_Existing_Assignment()
    {
        var custId = await SeedCustomerAsync("C007");
        var list1Id = await CreatePriceListAsync("List 1");
        var list2Id = await CreatePriceListAsync("List 2");

        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = list1Id });
        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = list2Id });

        var assignment = await _assignmentService.GetAssignmentAsync(custId);
        assignment.PriceListId.ShouldBe(list2Id);
    }

    [Fact]
    public async Task RemoveAssignment_Clears_Customer_Override()
    {
        var custId = await SeedCustomerAsync("C008");
        var listId = await CreatePriceListAsync("Temp List");

        await _assignmentService.AssignToCustomerAsync(new AssignPriceListDto { CustomerId = custId, PriceListId = listId });
        await _assignmentService.RemoveAssignmentAsync(custId);

        var assignment = await _assignmentService.GetAssignmentAsync(custId);
        assignment.ShouldBeNull();
    }
}
