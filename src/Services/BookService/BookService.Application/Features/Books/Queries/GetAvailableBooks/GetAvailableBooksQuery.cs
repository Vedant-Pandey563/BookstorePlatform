using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetAvailableBooks;

public sealed record GetAvailableBooksQuery : IRequest<List<BookDto>>;
