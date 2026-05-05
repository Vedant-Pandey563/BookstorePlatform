using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetAllOrders;

public sealed record GetAllOrdersQuery() : IRequest<List<OrderDto>>;
