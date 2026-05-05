using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

// Response model for orders.
// Keep it separate from the domain entity so the API shape stays stable.
public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
