using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrdersByStatus;

public sealed class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersByStatusQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetByStatusAsync(request.Status, cancellationToken);
        return orders.Select(x => x.ToDto()).ToList();
    }
}
