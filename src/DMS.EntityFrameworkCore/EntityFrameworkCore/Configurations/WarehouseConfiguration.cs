using DMS.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(Warehouse.MaxNameLength);

        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(Warehouse.MaxCodeLength);

        builder.Property(w => w.Data)
            .HasMaxLength(Warehouse.MaxDataLength);

        builder.Property(w => w.Street)
            .HasMaxLength(Warehouse.MaxStreetLength);

        builder.Property(w => w.Landmark)
            .HasMaxLength(Warehouse.MaxLandmarkLength);

        builder.Property(w => w.Latitude)
            .HasMaxLength(Warehouse.MaxCoordLength);

        builder.Property(w => w.Longitude)
            .HasMaxLength(Warehouse.MaxCoordLength);

        builder.Property(w => w.BuildingData)
            .HasMaxLength(Warehouse.MaxBuildingDataLength);
    }
}
