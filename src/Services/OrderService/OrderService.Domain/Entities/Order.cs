using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

// Aggregate root for the service.
// This object only owns order data, not cart/book/user/payment ownership.
public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string ShippingAddress { get; set; } = string.Empty;

    // A separate field is useful when payment service updates the payment result later.
    public string PaymentStatus { get; set; } = "Pending";

    // Used to prevent duplicate order creation on retries.
    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedUtc { get; set; }

    // Composition: an order contains order items.
    public List<OrderItem> Items { get; set; } = new();
}
