using BookService.Application.Abstractions;
using MediatR;

namespace BookService.Application.Features.Books.Commands.DeleteBook;

public sealed class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IBookRepository _repository;

    public DeleteBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.BookId, cancellationToken);
    }
}
