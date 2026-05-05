using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    int OrderId,
    OrderStatus NewStatus) : IRequest<OrderDto?>;
