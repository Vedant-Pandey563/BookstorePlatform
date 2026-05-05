using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Common;
using OrderService.Application.DTOs;
using OrderService.Domain.Rules;

namespace OrderService.Application.Features.Orders.Commands.CreateOrderFromCart;

public sealed class CreateOrderFromCartCommandHandler : IRequestHandler<CreateOrderFromCartCommand, OrderDto>
{
    private readonly IOrderRepository _repository;

    public CreateOrderFromCartCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
    {
        var order = request.ToDomain();

        OrderRules.EnsureHasItems(order.Items);
        OrderRules.EnsureValidItemQuantities(order.Items);

        order.TotalAmount = OrderRules.CalculateTotal(order.Items);
        order.OrderDate = DateTime.UtcNow;

        var created = await _repository.CreateAsync(order, cancellationToken);

        if (created is null)
        {
            throw new InvalidOperationException("Order creation from cart failed.");
        }

        return created.ToDto();
    }
}
