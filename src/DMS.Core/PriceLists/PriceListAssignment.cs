using Abp.Domain.Entities;

namespace DMS.PriceLists;

public class PriceListAssignment : Entity<int>, IMustHaveTenant
{
    public int TenantId { get; set; }
    public int PriceListId { get; set; }
    public int CustomerId { get; set; }

    public virtual PriceList PriceList { get; set; }
}
