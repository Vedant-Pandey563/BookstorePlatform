using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.UpdateBookStock;

public sealed class UpdateBookStockCommandHandler : IRequestHandler<UpdateBookStockCommand, BookDto?>
{
    private readonly IBookRepository _repository;

    public UpdateBookStockCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(UpdateBookStockCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.BookId, includeInactive: true, cancellationToken);
        if (book is null)
        {
            return null;
        }

        book.StockQuantity = request.StockQuantity;
        book.IsAvailable = request.StockQuantity > 0;
        book.UpdatedUtc = DateTime.UtcNow;

        var updated = await _repository.UpdateStockAsync(book.BookId, book.StockQuantity, cancellationToken);
        if (!updated)
        {
            return null;
        }

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
