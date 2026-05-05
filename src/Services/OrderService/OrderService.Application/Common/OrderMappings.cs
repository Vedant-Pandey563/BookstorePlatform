using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Domain.Entities;

namespace OrderService.Application.Common;

// Small mapping helpers keep handlers clean and readable.
public static class OrderMappings
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            PaymentStatus = order.PaymentStatus,
            IdempotencyKey = order.IdempotencyKey,
            CreatedUtc = order.CreatedUtc,
            UpdatedUtc = order.UpdatedUtc,
            Items = order.Items.Select(x => x.ToDto()).ToList()
        };
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto
        {
            OrderItemId = item.OrderItemId,
            OrderId = item.OrderId,
            BookId = item.BookId,
            BookTitle = item.BookTitle,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            LineTotal = item.LineTotal
        };
    }

    public static Order ToDomain(this CreateOrderCommandBase request)
    {
        return new Order
        {
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress.Trim(),
            IdempotencyKey = request.IdempotencyKey.Trim(),
            Items = request.Items.Select(x => new OrderItem
            {
                BookId = x.BookId,
                BookTitle = x.BookTitle.Trim(),
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                LineTotal = x.UnitPrice * x.Quantity
            }).ToList()
        };
    }
}
