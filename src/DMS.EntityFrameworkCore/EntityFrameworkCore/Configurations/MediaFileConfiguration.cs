using DMS.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");

        builder.Property(m => m.FileName).IsRequired().HasMaxLength(MediaFile.MaxFileNameLength);
        builder.Property(m => m.FilePath).IsRequired().HasMaxLength(MediaFile.MaxFilePathLength);
        builder.Property(m => m.ContentType).HasMaxLength(MediaFile.MaxContentTypeLength);
        builder.Property(m => m.MediaType).HasConversion<int>();

        builder.HasIndex(m => new { m.TenantId, m.MediaType, m.ModelId });
    }
}
