using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.DeactivateBook;

public sealed class DeactivateBookCommandHandler : IRequestHandler<DeactivateBookCommand, BookDto?>
{
    private readonly IBookRepository _repository;

    public DeactivateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(DeactivateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.BookId, includeInactive: true, cancellationToken);
        if (book is null)
        {
            return null;
        }

        var updated = await _repository.DeactivateAsync(request.BookId, cancellationToken);
        if (!updated)
        {
            return null;
        }

        book.IsActive = false;
        book.IsAvailable = false;
        book.UpdatedUtc = DateTime.UtcNow;

        return new BookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            ISBN = book.ISBN,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            Language = book.Language,
            PublishedDate = book.PublishedDate,
            IsAvailable = book.IsAvailable,
            IsActive = book.IsActive,
            CreatedUtc = book.CreatedUtc,
            UpdatedUtc = book.UpdatedUtc
        };
    }
}
