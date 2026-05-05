using BookService.Application.Abstractions;
using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByAuthor;

public sealed class GetBooksByAuthorQueryHandler : IRequestHandler<GetBooksByAuthorQuery, List<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBooksByAuthorQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookDto>> Handle(GetBooksByAuthorQuery request, CancellationToken cancellationToken)
    {
        var books = await _repository.SearchAsync(author: request.Author, cancellationToken: cancellationToken);

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
