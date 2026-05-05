using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

// Standard checkout request.
public sealed record CreateOrderCommand(
    int UserId,
    string ShippingAddress,
    string IdempotencyKey,
    List<CreateOrderItemDto> Items) : CreateOrderCommandBase(UserId, ShippingAddress, IdempotencyKey, Items), IRequest<OrderDto>;
