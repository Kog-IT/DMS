using DMS.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class RouteItemConfiguration : IEntityTypeConfiguration<RouteItem>
{
    public void Configure(EntityTypeBuilder<RouteItem> builder)
    {
        builder.ToTable("RouteItems");

        builder.HasIndex(i => i.RouteId);
        builder.HasIndex(i => new { i.RouteId, i.OrderIndex }).IsUnique();
        builder.HasIndex(i => i.TenantId);
    }
}
