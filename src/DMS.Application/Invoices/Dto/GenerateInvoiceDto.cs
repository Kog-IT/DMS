using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace DMS.Invoices.Dto;

public class GenerateInvoiceDto : IEntityDto<int>
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }
}
