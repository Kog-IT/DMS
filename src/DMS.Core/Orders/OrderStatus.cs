namespace DMS.Orders;

public enum OrderStatus
{
    Draft = 0,
    PendingApproval = 1,
    Confirmed = 2,
    PartiallyDelivered = 3,
    Delivered = 4,
    Cancelled = 5
}
