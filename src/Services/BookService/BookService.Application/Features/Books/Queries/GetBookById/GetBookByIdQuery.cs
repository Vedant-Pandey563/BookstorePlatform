using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(string BookId, bool IncludeInactive = false) : IRequest<BookDto?>;
