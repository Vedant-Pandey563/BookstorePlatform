using OrderService.Application.Common;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

// Shared request shape used by CreateOrder and CreateOrderFromCart.
public abstract record CreateOrderCommandBase(
    int UserId,
    string ShippingAddress,
    string IdempotencyKey,
    List<CreateOrderItemDto> Items)
{
    public Order ToDomain() => OrderMappings.ToDomain(this);
}
