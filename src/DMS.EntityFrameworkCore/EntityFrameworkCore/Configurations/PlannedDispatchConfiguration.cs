using DMS.Dispatches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class PlannedDispatchConfiguration : IEntityTypeConfiguration<PlannedDispatch>
{
    public void Configure(EntityTypeBuilder<PlannedDispatch> builder)
    {
        builder.ToTable("PlannedDispatches");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
