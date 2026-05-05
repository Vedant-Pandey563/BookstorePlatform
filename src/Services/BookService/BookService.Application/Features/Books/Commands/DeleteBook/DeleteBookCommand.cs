using MediatR;

namespace BookService.Application.Features.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(string BookId) : IRequest<bool>;
