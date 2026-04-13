using DMS.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(Route.MaxNameLength);

        builder.Property(r => r.Notes)
            .HasMaxLength(Route.MaxNotesLength);

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.HasMany(r => r.Items)
            .WithOne(i => i.Route)
            .HasForeignKey(i => i.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.TenantId, r.AssignedUserId });
        builder.HasIndex(r => r.PlannedDate);
    }
}
