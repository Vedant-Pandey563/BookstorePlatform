using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using BookService.Domain.Entities;
using MediatR;

namespace BookService.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto?>
{
    private readonly IBookRepository _repository;

    public UpdateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.BookId, includeInactive: true, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var isbnOwner = await _repository.GetByISBNAsync(request.ISBN.Trim(), cancellationToken);
        if (isbnOwner is not null && isbnOwner.BookId != request.BookId)
        {
            throw new InvalidOperationException($"Another book already uses ISBN '{request.ISBN}'.");
        }

        existing.Title = request.Title.Trim();
        existing.ISBN = request.ISBN.Trim();
        existing.Author = request.Author.Trim();
        existing.Description = request.Description?.Trim() ?? string.Empty;
        existing.Genre = request.Genre.Trim();
        existing.Price = request.Price;
        existing.StockQuantity = request.StockQuantity;
        existing.Language = request.Language?.Trim() ?? string.Empty;
        existing.PublishedDate = request.PublishedDate;
        existing.IsAvailable = request.StockQuantity > 0;
        existing.UpdatedUtc = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        if (!updated)
        {
            return null;
        }

        return new BookDto
        {
            BookId = existing.BookId,
            Title = existing.Title,
            ISBN = existing.ISBN,
            Author = existing.Author,
            Description = existing.Description,
            Genre = existing.Genre,
            Price = existing.Price,
            StockQuantity = existing.StockQuantity,
            Language = existing.Language,
            PublishedDate = existing.PublishedDate,
            IsAvailable = existing.IsAvailable,
            IsActive = existing.IsActive,
            CreatedUtc = existing.CreatedUtc,
            UpdatedUtc = existing.UpdatedUtc
        };
    }
}
