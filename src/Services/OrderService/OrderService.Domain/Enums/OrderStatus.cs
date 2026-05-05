namespace OrderService.Domain.Enums;

// Explicit lifecycle for order processing.
// Keep the enum in Domain so status rules can be enforced centrally.
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    PaymentPending = 2,
    Paid = 3,
    Cancelled = 4,
    Shipped = 5,
    Delivered = 6
}
