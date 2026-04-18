using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DMS.Customers;

namespace DMS.Customers.Dto;

[AutoMapFrom(typeof(CustomerContact))]
public class CustomerContactDto : FullAuditedEntityDto<int>
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string WhatsApp { get; set; }
    public string SocialHandle { get; set; }
    public bool IsPrimary { get; set; }
}
