using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Queries.GetOrdersByStatus;

public sealed record GetOrdersByStatusQuery(OrderStatus Status) : IRequest<List<OrderDto>>;
