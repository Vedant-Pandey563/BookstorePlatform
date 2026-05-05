using FluentValidation;

namespace OrderService.Application.Features.Orders.Commands.CreateOrderFromCart;

public sealed class CreateOrderFromCartCommandValidator : AbstractValidator<CreateOrderFromCartCommand>
{
    public CreateOrderFromCartCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(300);
        RuleFor(x => x.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.BookId).GreaterThan(0);
            item.RuleFor(i => i.BookTitle).NotEmpty().MaximumLength(200);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
