using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _repository;

    public GetBookByIdQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.BookId, request.IncludeInactive, cancellationToken);

        return book is null
            ? null
            : new BookDto
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
