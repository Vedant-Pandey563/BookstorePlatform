using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrdersByUserId;

public sealed class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, List<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersByUserIdQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        return orders.Select(x => x.ToDto()).ToList();
    }
}
