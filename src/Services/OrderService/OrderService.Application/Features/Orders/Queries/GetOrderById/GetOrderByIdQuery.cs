using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(int OrderId) : IRequest<OrderDto?>;
