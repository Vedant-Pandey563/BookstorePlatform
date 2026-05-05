using BookService.Domain.Entities;

namespace BookService.Application.Abstractions;

// Repository abstraction used by CQRS handlers.
// Application knows only about this interface, not about MongoDB.
public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(string bookId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Book?> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default);

    Task<List<Book>> SearchAsync(
        string? title = null,
        string? author = null,
        string? genre = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool availableOnly = false,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<string> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string bookId, CancellationToken cancellationToken = default);
    Task<bool> UpdateStockAsync(string bookId, int stockQuantity, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(string bookId, CancellationToken cancellationToken = default);
}
