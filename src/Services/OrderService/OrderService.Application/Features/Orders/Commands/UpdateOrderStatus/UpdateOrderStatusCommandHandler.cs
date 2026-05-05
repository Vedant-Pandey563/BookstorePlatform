using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;
using OrderService.Domain.Rules;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto?>
{
    private readonly IOrderRepository _repository;

    public UpdateOrderStatusCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto?> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        // Validate the business transition before saving.
        if (!OrderRules.CanTransition(order.Status, request.NewStatus))
        {
            throw new InvalidOperationException(
                $"Invalid order status transition: {order.Status} -> {request.NewStatus}");
        }

        var updated = await _repository.UpdateStatusAsync(request.OrderId, request.NewStatus, cancellationToken);
        if (!updated)
        {
            return null;
        }

        order.Status = request.NewStatus;
        order.UpdatedUtc = DateTime.UtcNow;

        return order.ToDto();
    }
}
