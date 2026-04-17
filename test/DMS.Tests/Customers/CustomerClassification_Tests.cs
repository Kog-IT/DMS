using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMS.Customers;
using DMS.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Customers;

public class CustomerClassification_Tests
{
    private readonly CustomerClassificationService _service = new();

    [Fact]
    public void Classify_Empty_Customers_Returns_Empty_Result()
    {
        var result = _service.Classify(Array.Empty<(int, int)>(), 8, 4, 10);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Classify_Uses_Threshold_Mode_When_Enough_Customers()
    {
        var customers = new List<(int, int)>();
        for (int i = 1; i <= 10; i++)
            customers.Add((i, i));

        var result = _service.Classify(customers, 8, 4, 10);
        result.Find(r => r.CustomerId == 10)!.Classification.ShouldBe(CustomerClassification.A);
        result.Find(r => r.CustomerId == 5)!.Classification.ShouldBe(CustomerClassification.B);
        result.Find(r => r.CustomerId == 3)!.Classification.ShouldBe(CustomerClassification.C);
    }

    [Fact]
    public void Classify_A_When_Visits_Meet_ThresholdA()
    {
        var customers = BuildCustomers(12, 8, 4, 1);
        var result = _service.Classify(customers, 8, 4, 10);
        result.Find(r => r.CustomerId == 1)!.Classification.ShouldBe(CustomerClassification.A);
    }

    [Fact]
    public void Classify_B_When_Visits_Between_Thresholds()
    {
        var customers = BuildCustomers(12, 8, 4, 1);
        var result = _service.Classify(customers, 8, 4, 10);
        result.Find(r => r.CustomerId == 2)!.Classification.ShouldBe(CustomerClassification.B);
    }

    [Fact]
    public void Classify_C_When_Visits_Below_ThresholdB()
    {
        var customers = BuildCustomers(12, 8, 4, 1);
        var result = _service.Classify(customers, 8, 4, 10);
        result.Find(r => r.CustomerId == 3)!.Classification.ShouldBe(CustomerClassification.C);
    }

    [Fact]
    public void Classify_Zero_Visits_Gets_Class_C()
    {
        var customers = new List<(int CustomerId, int VisitCount)>
        {
            (1, 0), (2, 0), (3, 0), (4, 0), (5, 0),
            (6, 0), (7, 0), (8, 0), (9, 0), (10, 0),
        };
        var result = _service.Classify(customers, 8, 4, 10);
        result.ForEach(r => r.Classification.ShouldBe(CustomerClassification.C));
    }

    [Fact]
    public void Classify_Uses_Percentile_Mode_When_Below_MinCustomers()
    {
        var customers = new List<(int CustomerId, int VisitCount)>
        {
            (1, 100), (2, 50), (3, 30), (4, 10), (5, 1)
        };
        var result = _service.Classify(customers, 8, 4, 10);
        result.Find(r => r.CustomerId == 1)!.Classification.ShouldBe(CustomerClassification.A);
        result.Find(r => r.CustomerId == 2)!.Classification.ShouldBe(CustomerClassification.B);
        result.Find(r => r.CustomerId == 3)!.Classification.ShouldBe(CustomerClassification.C);
        result.Find(r => r.CustomerId == 4)!.Classification.ShouldBe(CustomerClassification.C);
        result.Find(r => r.CustomerId == 5)!.Classification.ShouldBe(CustomerClassification.C);
    }

    private static List<(int CustomerId, int VisitCount)> BuildCustomers(
        int n, params int[] visitCounts)
    {
        var list = new List<(int, int)>();
        for (int i = 0; i < n; i++)
            list.Add((i + 1, visitCounts[i % visitCounts.Length]));
        return list;
    }
}

public class CustomerClassificationIntegration_Tests : DMSTestBase
{
    private readonly IClassifyCustomersAppService _classifyService;

    public CustomerClassificationIntegration_Tests()
    {
        _classifyService = Resolve<IClassifyCustomersAppService>();
    }

    [Fact]
    public async Task Classify_Updates_Customer_Entity_In_Database()
    {
        // Need 10+ customers to trigger threshold mode (MinCustomersForPercentile = 10)
        // The test customer has 0 visits → below thresholdB (4) → class C
        int customerId = 0;
        await UsingDbContextAsync(async ctx =>
        {
            // Add 9 filler customers with high visit counts (no actual visits, but threshold mode
            // is determined by count only; visits are queried separately — 0 visits for all)
            for (int i = 1; i <= 9; i++)
                ctx.Set<Customer>().Add(new Customer { TenantId = 1, Code = $"INT1_F{i:D2}", Name = $"Filler {i}", IsActive = true });

            var customer = new Customer
            {
                TenantId = 1,
                Code = "INT1_C001",
                Name = "Test Customer",
                IsActive = true,
                Classification = CustomerClassification.Unclassified
            };
            ctx.Set<Customer>().Add(customer);
            await ctx.SaveChangesAsync();
            customerId = customer.Id;
        });

        LoginAsDefaultTenantAdmin();
        var results = await _classifyService.ClassifyAllAsync();

        results.ShouldNotBeEmpty();
        await UsingDbContextAsync(async ctx =>
        {
            var customer = await ctx.Set<Customer>().FindAsync(customerId);
            customer.Classification.ShouldNotBe(CustomerClassification.Unclassified);
            customer.Classification.ShouldBe(CustomerClassification.C);
        });
    }

    [Fact]
    public async Task Classify_Sets_LastClassifiedAt()
    {
        await UsingDbContextAsync(async ctx =>
        {
            for (int i = 1; i <= 9; i++)
                ctx.Set<Customer>().Add(new Customer { TenantId = 1, Code = $"INT2_F{i:D2}", Name = $"Filler {i}", IsActive = true });
            ctx.Set<Customer>().Add(new Customer
            {
                TenantId = 1, Code = "INT2_C002", Name = "Customer2",
                IsActive = true, Classification = CustomerClassification.Unclassified
            });
            await ctx.SaveChangesAsync();
        });

        LoginAsDefaultTenantAdmin();
        await _classifyService.ClassifyAllAsync();

        await UsingDbContextAsync(async ctx =>
        {
            var customer = await ctx.Set<Customer>()
                .FirstOrDefaultAsync(c => c.Code == "INT2_C002" && c.TenantId == 1);
            customer.LastClassifiedAt.ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task Classify_Returns_OldClass_And_NewClass_In_Result()
    {
        await UsingDbContextAsync(async ctx =>
        {
            for (int i = 1; i <= 9; i++)
                ctx.Set<Customer>().Add(new Customer { TenantId = 1, Code = $"INT3_F{i:D2}", Name = $"Filler {i}", IsActive = true });
            ctx.Set<Customer>().Add(new Customer
            {
                TenantId = 1, Code = "INT3_C003", Name = "Customer3",
                IsActive = true, Classification = CustomerClassification.A
            });
            await ctx.SaveChangesAsync();
        });

        LoginAsDefaultTenantAdmin();
        var results = await _classifyService.ClassifyAllAsync();

        var result = results.Find(r => r.CustomerName == "Customer3");
        result.ShouldNotBeNull();
        result.OldClassification.ShouldBe(CustomerClassification.A);
        result.NewClassification.ShouldBe(CustomerClassification.C);
        result.Changed.ShouldBeTrue();
    }
}
