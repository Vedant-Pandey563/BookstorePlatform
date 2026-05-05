using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrderItems;

public sealed record GetOrderItemsQuery(int OrderId) : IRequest<List<OrderItemDto>>;
