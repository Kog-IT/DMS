using DMS.Salesmen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class SalesmanConfiguration : IEntityTypeConfiguration<Salesman>
{
    public void Configure(EntityTypeBuilder<Salesman> builder)
    {
        builder.ToTable("Salesmen");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(Salesman.MaxNameLength);

        builder.Property(s => s.Code)
            .HasMaxLength(Salesman.MaxCodeLength);

        builder.Property(s => s.Mobile)
            .HasMaxLength(Salesman.MaxMobileLength);

        builder.Property(s => s.NationalId)
            .HasMaxLength(Salesman.MaxNationalIdLength);

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(Salesman.MaxImageUrlLength);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.AssignedWarehouseId)
            .IsRequired(false);
    }
}
