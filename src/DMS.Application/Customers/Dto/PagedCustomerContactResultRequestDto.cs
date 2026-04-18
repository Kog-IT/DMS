using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace DMS.Customers.Dto;

public class PagedCustomerContactResultRequestDto : PagedResultRequestDto
{
    [Required]
    public int CustomerId { get; set; }
}
