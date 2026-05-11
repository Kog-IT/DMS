using DMS.Dispatches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class ActualDispatchConfiguration : IEntityTypeConfiguration<ActualDispatch>
{
    public void Configure(EntityTypeBuilder<ActualDispatch> builder)
    {
        builder.ToTable("ActualDispatches");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,4)")
            .HasDefaultValue(0);
    }
}
