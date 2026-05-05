using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByAuthor;

public sealed record GetBooksByAuthorQuery(string Author) : IRequest<List<BookDto>>;
