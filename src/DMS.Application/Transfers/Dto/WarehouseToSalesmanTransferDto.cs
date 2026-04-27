using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Transfers.Dto;

public class WarehouseToSalesmanTransferDto : EntityDto<int>
{
    public int WarehouseId { get; set; }
    public int SalesmanId { get; set; }
    public DateTime TransferDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public string RejectionReason { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}
