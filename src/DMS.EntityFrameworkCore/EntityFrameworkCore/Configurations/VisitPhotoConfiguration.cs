using DMS.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class VisitPhotoConfiguration : IEntityTypeConfiguration<VisitPhoto>
{
    public void Configure(EntityTypeBuilder<VisitPhoto> builder)
    {
        builder.ToTable("VisitPhotos");

        builder.Property(p => p.FilePath)
            .IsRequired()
            .HasMaxLength(VisitPhoto.MaxFilePathLength);

        builder.Property(p => p.Caption)
            .HasMaxLength(VisitPhoto.MaxCaptionLength);

        builder.HasIndex(p => p.VisitId);
        builder.HasIndex(p => p.TenantId);
    }
}
