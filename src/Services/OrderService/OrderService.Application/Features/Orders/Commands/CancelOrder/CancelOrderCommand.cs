using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(int OrderId) : IRequest<OrderDto?>;
