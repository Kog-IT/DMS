using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers.Dto;

[AutoMapTo(typeof(CustomerContact))]
public class UpdateCustomerContactDto : EntityDto<int>
{
    [Required]
    [StringLength(CustomerContact.MaxNameLength)]
    public string Name { get; set; }

    [StringLength(CustomerContact.MaxPhoneLength)]
    public string Phone { get; set; }

    [StringLength(CustomerContact.MaxEmailLength)]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(CustomerContact.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(CustomerContact.MaxWhatsAppLength)]
    public string WhatsApp { get; set; }

    [StringLength(CustomerContact.MaxSocialHandleLength)]
    public string SocialHandle { get; set; }

    public bool IsPrimary { get; set; }
}
