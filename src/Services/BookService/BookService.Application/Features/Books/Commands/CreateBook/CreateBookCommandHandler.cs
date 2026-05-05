using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using BookService.Domain.Entities;
using MediatR;

namespace BookService.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IBookRepository _repository;

    public CreateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // ISBN must be unique.
        var existing = await _repository.GetByISBNAsync(request.ISBN.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"A book with ISBN '{request.ISBN}' already exists.");
        }

        var book = new Book
        {
            BookId = Guid.NewGuid().ToString("N"),
            Title = request.Title.Trim(),
            ISBN = request.ISBN.Trim(),
            Author = request.Author.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Genre = request.Genre.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Language = request.Language?.Trim() ?? string.Empty,
            PublishedDate = request.PublishedDate,
            IsAvailable = request.StockQuantity > 0,
            IsActive = true,
            CreatedUtc = DateTime.UtcNow
        };

        var newId = await _repository.AddAsync(book, cancellationToken);
        book.BookId = newId;

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
