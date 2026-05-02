using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace DMS.Transfers.Dto;

public class SalesmanToWarehouseTransferDto : EntityDto<int>
{
    public int SalesmanId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime TransferDate { get; set; }
    public string Notes { get; set; }
    public int Status { get; set; }
    public string RejectionReason { get; set; }
    public string TransactionNumber { get; set; }
    public string SalesManName { get; set; }
    public string WarehouseName { get; set; }
    public int TransactionStatus => Status;
    public int ProductsQuantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}
