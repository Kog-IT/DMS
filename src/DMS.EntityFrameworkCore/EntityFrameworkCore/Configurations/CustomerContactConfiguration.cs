using DMS.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.EntityFrameworkCore.Configurations;

public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("CustomerContacts");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(CustomerContact.MaxNameLength);

        builder.Property(c => c.Phone)
            .HasMaxLength(CustomerContact.MaxPhoneLength);

        builder.Property(c => c.Email)
            .HasMaxLength(CustomerContact.MaxEmailLength);

        builder.Property(c => c.Title)
            .HasMaxLength(CustomerContact.MaxTitleLength);

        builder.Property(c => c.WhatsApp)
            .HasMaxLength(CustomerContact.MaxWhatsAppLength);

        builder.Property(c => c.SocialHandle)
            .HasMaxLength(CustomerContact.MaxSocialHandleLength);

        builder.Property(c => c.IsPrimary)
            .HasDefaultValue(false);

        builder.HasIndex(c => new { c.TenantId, c.CustomerId });

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
