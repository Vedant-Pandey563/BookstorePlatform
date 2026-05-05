using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string ISBN,
    string Author,
    string Description,
    string Genre,
    decimal Price,
    int StockQuantity,
    string Language,
    DateTime PublishedDate) : IRequest<BookDto>;
