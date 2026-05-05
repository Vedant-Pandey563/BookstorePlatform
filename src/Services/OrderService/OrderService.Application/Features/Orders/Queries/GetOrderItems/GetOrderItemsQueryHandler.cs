using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrderItems;

public sealed class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, List<OrderItemDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrderItemsQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderItemDto>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsByOrderIdAsync(request.OrderId, cancellationToken);
        return items.Select(x => x.ToDto()).ToList();
    }
}
