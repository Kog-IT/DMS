using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Transfers.Dto;

public class WarehouseToWarehouseTransferDto : EntityDto<int>
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime TransferDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}
