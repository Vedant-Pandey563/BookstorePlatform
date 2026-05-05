using FluentValidation;

namespace BookService.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        // Required text fields.
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(100);

        // Business rules from the assignment.
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);

        // Optional fields are still bounded so data stays reasonable.
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Language).MaximumLength(50);
    }
}
