using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery(bool IncludeInactive = false) : IRequest<List<BookDto>>;
