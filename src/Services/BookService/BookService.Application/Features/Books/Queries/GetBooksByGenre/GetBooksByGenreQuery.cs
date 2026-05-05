using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByGenre;

public sealed record GetBooksByGenreQuery(string Genre) : IRequest<List<BookDto>>;
