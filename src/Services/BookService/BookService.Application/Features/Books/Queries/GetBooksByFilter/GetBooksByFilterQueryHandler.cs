using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByFilter;

public sealed class GetBooksByFilterQueryHandler : IRequestHandler<GetBooksByFilterQuery, List<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBooksByFilterQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookDto>> Handle(GetBooksByFilterQuery request, CancellationToken cancellationToken)
    {
        var books = await _repository.SearchAsync(
            author: request.Author,
            genre: request.Genre,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            includeInactive: request.IncludeInactive,
            cancellationToken: cancellationToken);

        return books.Select(book => new BookDto
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
        }).ToList();
    }
}
