using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrdersByUserId;

public sealed record GetOrdersByUserIdQuery(int UserId) : IRequest<List<OrderDto>>;
