using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Transfers.Dto;

public class CreateSalesmanToWarehouseTransferDto
{
    [Required]
    public int SalesmanId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    public DateTime TransferDate { get; set; }

    public string Notes { get; set; }

    public List<CreateTransferItemDto> Items { get; set; } = new();
}
