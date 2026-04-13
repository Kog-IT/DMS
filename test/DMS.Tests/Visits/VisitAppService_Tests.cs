using Abp.Application.Services.Dto;
using DMS.Customers;
using DMS.Customers.Dto;
using DMS.Visits;
using DMS.Visits.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DMS.Tests.Visits;

public class VisitAppService_Tests : DMSTestBase
{
    private readonly IVisitAppService _visitAppService;
    private readonly ICustomerAppService _customerAppService;

    public VisitAppService_Tests()
    {
        _visitAppService = Resolve<IVisitAppService>();
        _customerAppService = Resolve<ICustomerAppService>();
    }

    private async Task<CustomerDto> CreateCustomerAtLocation(double lat, double lon)
    {
        return await _customerAppService.CreateAsync(new CreateCustomerDto
        {
            Code = $"C-{Guid.NewGuid():N}".Substring(0, 10),
            Name = "Test Customer",
            Latitude = lat,
            Longitude = lon,
            IsActive = true
        });
    }

    private async Task<VisitDto> CreatePlannedVisit(int customerId)
    {
        return await _visitAppService.CreateAsync(new CreateVisitDto
        {
            CustomerId = customerId,
            AssignedUserId = AbpSession.UserId.Value,
            PlannedDate = DateTime.Today
        });
    }

    [Fact]
    public async Task GetAll_Returns_Empty_For_New_Tenant()
    {
        var result = await _visitAppService.GetAllAsync(
            new PagedVisitResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Create_Visit_Has_Planned_Status()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit = await CreatePlannedVisit(customer.Id);

        visit.Id.ShouldBeGreaterThan(0);
        visit.Status.ShouldBe(VisitStatus.Planned);
        visit.CustomerId.ShouldBe(customer.Id);
    }

    [Fact]
    public async Task CheckIn_Sets_InProgress_Status()
    {
        var customer = await CreateCustomerAtLocation(30.0444, 31.2357);
        var visit = await CreatePlannedVisit(customer.Id);

        var result = await _visitAppService.CheckInAsync(new CheckInDto
        {
            VisitId = visit.Id,
            Latitude = 30.0444,
            Longitude = 31.2357
        });

        result.Status.ShouldBe(VisitStatus.InProgress);
        result.CheckInTime.ShouldNotBeNull();
        result.CheckInLatitude.ShouldBe(30.0444);
    }

    [Fact]
    public async Task CheckOut_Sets_Completed_Status_And_Computes_Duration()
    {
        var customer = await CreateCustomerAtLocation(30.0444, 31.2357);
        var visit = await CreatePlannedVisit(customer.Id);

        await _visitAppService.CheckInAsync(new CheckInDto
        {
            VisitId = visit.Id,
            Latitude = 30.0444,
            Longitude = 31.2357
        });

        var result = await _visitAppService.CheckOutAsync(new CheckOutDto
        {
            VisitId = visit.Id,
            Latitude = 30.0444,
            Longitude = 31.2357,
            Notes = "All good",
            NoSaleReason = null
        });

        result.Status.ShouldBe(VisitStatus.Completed);
        result.CheckOutTime.ShouldNotBeNull();
        result.DurationMinutes.ShouldNotBeNull();
        result.DurationMinutes.Value.ShouldBeGreaterThanOrEqualTo(0);
        result.Notes.ShouldBe("All good");
    }

    [Fact]
    public async Task CheckOut_Requires_InProgress_Status()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit = await CreatePlannedVisit(customer.Id);

        await Should.ThrowAsync<Abp.UI.UserFriendlyException>(async () =>
        {
            await _visitAppService.CheckOutAsync(new CheckOutDto { VisitId = visit.Id });
        });
    }

    [Fact]
    public async Task Skip_Sets_Skipped_Status()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit = await CreatePlannedVisit(customer.Id);

        var result = await _visitAppService.SkipAsync(new SkipVisitDto
        {
            VisitId = visit.Id,
            SkipReason = "Customer closed"
        });

        result.Status.ShouldBe(VisitStatus.Skipped);
        result.SkipReason.ShouldBe("Customer closed");
    }

    [Fact]
    public async Task Skip_Requires_Reason()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit = await CreatePlannedVisit(customer.Id);

        // Empty string triggers ABP's [Required] validation before the service method runs
        await Should.ThrowAsync<Abp.Runtime.Validation.AbpValidationException>(async () =>
        {
            await _visitAppService.SkipAsync(new SkipVisitDto
            {
                VisitId = visit.Id,
                SkipReason = ""
            });
        });
    }

    [Fact]
    public async Task GetAll_Filters_By_Status()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit1 = await CreatePlannedVisit(customer.Id);
        var visit2 = await CreatePlannedVisit(customer.Id);

        await _visitAppService.SkipAsync(new SkipVisitDto
        {
            VisitId = visit1.Id,
            SkipReason = "No access"
        });

        var skipped = await _visitAppService.GetAllAsync(
            new PagedVisitResultRequestDto
            {
                MaxResultCount = 20,
                SkipCount = 0,
                Status = VisitStatus.Skipped
            });

        skipped.Items.ShouldAllBe(v => v.Status == VisitStatus.Skipped);
        skipped.Items.ShouldContain(v => v.Id == visit1.Id);
        skipped.Items.ShouldNotContain(v => v.Id == visit2.Id);
    }

    [Fact]
    public async Task Tenant_Isolation_Visit_TenantId_Stamped_Correctly()
    {
        var customer = await CreateCustomerAtLocation(30.0, 31.0);
        var visit = await CreatePlannedVisit(customer.Id);

        await UsingDbContextAsync(async context =>
        {
            var entity = await context.Set<DMS.Visits.Visit>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(v => v.Id == visit.Id);

            entity.ShouldNotBeNull();
            entity.TenantId.ShouldBe(AbpSession.TenantId.Value);
        });
    }
}
