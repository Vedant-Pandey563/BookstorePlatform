using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.SearchBooksByTitle;

public sealed record SearchBooksByTitleQuery(string Title) : IRequest<List<BookDto>>;
