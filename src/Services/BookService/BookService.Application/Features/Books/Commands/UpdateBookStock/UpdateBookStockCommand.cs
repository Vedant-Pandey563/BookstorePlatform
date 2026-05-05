using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.UpdateBookStock;

public sealed record UpdateBookStockCommand(string BookId, int StockQuantity) : IRequest<BookDto?>;
