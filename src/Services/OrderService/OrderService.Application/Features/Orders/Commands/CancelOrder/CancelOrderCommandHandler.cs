using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;
using OrderService.Domain.Rules;

namespace OrderService.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    private readonly IOrderRepository _repository;

    public CancelOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto?> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        if (!OrderRules.CanCancel(order.Status))
        {
            throw new InvalidOperationException("This order can no longer be cancelled.");
        }

        var cancelled = await _repository.CancelAsync(request.OrderId, cancellationToken);
        if (!cancelled)
        {
            return null;
        }

        order.Status = OrderStatus.Cancelled;
        order.PaymentStatus = "Cancelled";
        order.UpdatedUtc = DateTime.UtcNow;

        return order.ToDto();
    }
}
