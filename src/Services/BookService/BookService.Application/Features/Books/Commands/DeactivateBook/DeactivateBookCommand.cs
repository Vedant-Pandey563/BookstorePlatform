using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Commands.DeactivateBook;

public sealed record DeactivateBookCommand(string BookId) : IRequest<BookDto?>;
