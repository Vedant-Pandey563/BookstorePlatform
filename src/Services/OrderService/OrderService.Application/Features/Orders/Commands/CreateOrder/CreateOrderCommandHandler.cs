using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;
using OrderService.Domain.Rules;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = request.ToDomain();

        // Domain rules are still enforced in the application flow for fail-fast behavior.
        OrderRules.EnsureHasItems(order.Items);
        OrderRules.EnsureValidItemQuantities(order.Items);

        order.TotalAmount = OrderRules.CalculateTotal(order.Items);
        order.Status = Domain.Enums.OrderStatus.Pending;
        order.PaymentStatus = "Pending";
        order.OrderDate = DateTime.UtcNow;
        order.CreatedUtc = DateTime.UtcNow;

        var created = await _repository.CreateAsync(order, cancellationToken);

        if (created is null)
        {
            throw new InvalidOperationException("Order creation failed.");
        }

        return created.ToDto();
    }
}
