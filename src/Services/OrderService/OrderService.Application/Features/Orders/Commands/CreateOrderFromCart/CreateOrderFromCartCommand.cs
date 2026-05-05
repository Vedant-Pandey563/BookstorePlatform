using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CreateOrder;

namespace OrderService.Application.Features.Orders.Commands.CreateOrderFromCart;

// Same execution path as a normal create-order command,
// but the source of the item list would typically be cart checkout data.
public sealed record CreateOrderFromCartCommand(
    int UserId,
    string ShippingAddress,
    string IdempotencyKey,
    List<CreateOrderItemDto> Items) : CreateOrderCommandBase(UserId, ShippingAddress, IdempotencyKey, Items), IRequest<OrderDto>;
