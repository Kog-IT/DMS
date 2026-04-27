using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Transfers.Dto;

public class SalesmanToSalesmanTransferDto : EntityDto<int>
{
    public int FromSalesmanId { get; set; }
    public int ToSalesmanId { get; set; }
    public DateTime TransferDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public string RejectionReason { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}
