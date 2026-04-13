using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace DMS.Routes;

public class RouteItem : FullAuditedEntity<int>, IMustHaveTenant
{
    public int TenantId { get; set; }
    public int RouteId { get; set; }
    public virtual Route Route { get; set; }
    public int CustomerId { get; set; }
    public int OrderIndex { get; set; }
    public int? PlannedDurationMinutes { get; set; }
}
