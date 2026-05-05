using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByPriceRange;

public sealed class GetBooksByPriceRangeQueryHandler : IRequestHandler<GetBooksByPriceRangeQuery, List<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBooksByPriceRangeQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookDto>> Handle(GetBooksByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        var books = await _repository.SearchAsync(
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
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
