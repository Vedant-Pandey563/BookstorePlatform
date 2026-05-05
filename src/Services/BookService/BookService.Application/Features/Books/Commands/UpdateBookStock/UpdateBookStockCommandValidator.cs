using FluentValidation;

namespace BookService.Application.Features.Books.Commands.UpdateBookStock;

public sealed class UpdateBookStockCommandValidator : AbstractValidator<UpdateBookStockCommand>
{
    public UpdateBookStockCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
