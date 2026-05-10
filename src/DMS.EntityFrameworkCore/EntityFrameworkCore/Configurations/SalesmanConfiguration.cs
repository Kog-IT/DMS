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

        builder.Property(s => s.JobCode)
            .HasMaxLength(Salesman.MaxJobCodeLength);

        builder.Property(s => s.Mobile)
            .HasMaxLength(Salesman.MaxMobileLength);

        builder.Property(s => s.Email)
            .HasMaxLength(Salesman.MaxEmailLength);

        builder.Property(s => s.NationalNumber)
            .HasMaxLength(Salesman.MaxNationalNumberLength);

        builder.Property(s => s.Address)
            .HasMaxLength(Salesman.MaxAddressLength);

        builder.Property(s => s.Path)
            .HasMaxLength(Salesman.MaxPathLength);

        builder.Property(s => s.SalesSupervisorId)
            .HasMaxLength(Salesman.MaxSalesSupervisorIdLength);

        builder.Property(s => s.UserName)
            .HasMaxLength(Salesman.MaxUserNameLength);

        builder.Property(s => s.UserId)
            .IsRequired(false);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);
    }
}

public class SalesmanWarehouseConfiguration : IEntityTypeConfiguration<SalesmanWarehouse>
{
    public void Configure(EntityTypeBuilder<SalesmanWarehouse> builder)
    {
        builder.ToTable("SalesmanWarehouses");

        builder.HasIndex(sw => new { sw.SalesmanId, sw.WarehouseId })
            .IsUnique();
    }
}
