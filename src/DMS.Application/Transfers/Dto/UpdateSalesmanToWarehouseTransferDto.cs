using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Transfers.Dto;

public class UpdateSalesmanToWarehouseTransferDto : EntityDto<int>
{
    [Required]
    public int SalesmanId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    public DateTime TransferDate { get; set; }

    public string Notes { get; set; }

    public int Status { get; set; }

    public List<CreateTransferItemDto> Items { get; set; } = new();
}
