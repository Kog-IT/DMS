using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Transfers.Dto;

public class UpdateWarehouseToWarehouseTransferDto : EntityDto<int>
{
    [Required]
    public int FromWarehouseId { get; set; }

    [Required]
    public int ToWarehouseId { get; set; }

    [Required]
    public DateTime TransferDate { get; set; }

    public string Notes { get; set; }

    public int Status { get; set; }

    public List<CreateTransferItemDto> Items { get; set; } = new();
}
