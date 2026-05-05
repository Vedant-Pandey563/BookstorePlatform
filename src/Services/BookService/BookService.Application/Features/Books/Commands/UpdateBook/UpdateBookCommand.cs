using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(
    string BookId,
    string Title,
    string ISBN,
    string Author,
    string Description,
    string Genre,
    decimal Price,
    int StockQuantity,
    string Language,
    DateTime PublishedDate) : IRequest<BookDto?>;
