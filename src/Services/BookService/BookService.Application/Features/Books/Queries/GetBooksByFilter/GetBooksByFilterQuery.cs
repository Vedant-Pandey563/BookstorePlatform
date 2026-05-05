using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByFilter;

public sealed record GetBooksByFilterQuery(
    string? Author,
    string? Genre,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool IncludeInactive = false) : IRequest<List<BookDto>>;
