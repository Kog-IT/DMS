using Abp.Domain.Entities;

namespace DMS.SalesmanRequests;

public class SalesmanRequestItem : Entity<int>, IMustHaveTenant
{
    public int RequestId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public int TenantId { get; set; }
}
