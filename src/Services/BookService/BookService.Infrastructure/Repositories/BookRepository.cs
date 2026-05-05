using System.Text.RegularExpressions;
using BookService.Application.Abstractions;
using BookService.Domain.Entities;
using BookService.Infrastructure.Models;
using BookService.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookService.Infrastructure.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly IMongoCollection<BookDocument> _books;

    public BookRepository(IMongoDatabase database, IOptions<MongoSettings> options)
    {
        // The collection name comes from config so it is easy to change later.
        var settings = options.Value;
        _books = database.GetCollection<BookDocument>(settings.BooksCollectionName);
    }

    public async Task<List<Book>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var filter = includeInactive
            ? FilterDefinition<BookDocument>.Empty
            : Builders<BookDocument>.Filter.Eq(x => x.IsActive, true);

        var documents = await _books.Find(filter)
            .SortBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToDomain).ToList();
    }

    public async Task<Book?> GetByIdAsync(string bookId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var filter = Builders<BookDocument>.Filter.Eq(x => x.Id, bookId);

        if (!includeInactive)
        {
            filter = Builders<BookDocument>.Filter.And(
                filter,
                Builders<BookDocument>.Filter.Eq(x => x.IsActive, true));
        }

        var document = await _books.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : MapToDomain(document);
    }

    public async Task<Book?> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var normalized = isbn.Trim();

        var filter = Builders<BookDocument>.Filter.Eq(x => x.ISBN, normalized);
        var document = await _books.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : MapToDomain(document);
    }

    public async Task<List<Book>> SearchAsync(
        string? title = null,
        string? author = null,
        string? genre = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool availableOnly = false,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<BookDocument>>();
        var builder = Builders<BookDocument>.Filter;

        if (!includeInactive)
        {
            filters.Add(builder.Eq(x => x.IsActive, true));
        }

        if (availableOnly)
        {
            filters.Add(builder.Eq(x => x.IsAvailable, true));
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            filters.Add(builder.Regex(x => x.Title, new BsonRegularExpression(Regex.Escape(title.Trim()), "i")));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            filters.Add(builder.Regex(x => x.Author, new BsonRegularExpression(Regex.Escape(author.Trim()), "i")));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            filters.Add(builder.Regex(x => x.Genre, new BsonRegularExpression(Regex.Escape(genre.Trim()), "i")));
        }

        if (minPrice.HasValue)
        {
            filters.Add(builder.Gte(x => x.Price, minPrice.Value));
        }

        if (maxPrice.HasValue)
        {
            filters.Add(builder.Lte(x => x.Price, maxPrice.Value));
        }

        var finalFilter = filters.Count == 0 ? FilterDefinition<BookDocument>.Empty : builder.And(filters);
        var documents = await _books.Find(finalFilter).SortBy(x => x.Title).ToListAsync(cancellationToken);

        return documents.Select(MapToDomain).ToList();
    }

    public async Task<string> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(book.BookId))
        {
            book.BookId = Guid.NewGuid().ToString("N");
        }

        var document = MapToDocument(book);
        await _books.InsertOneAsync(document, cancellationToken: cancellationToken);
        return book.BookId;
    }

    public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        var document = MapToDocument(book);

        var result = await _books.ReplaceOneAsync(
            x => x.Id == book.BookId,
            document,
            cancellationToken: cancellationToken);

        return result.ModifiedCount > 0 || result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string bookId, CancellationToken cancellationToken = default)
    {
        var result = await _books.DeleteOneAsync(x => x.Id == bookId, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateStockAsync(string bookId, int stockQuantity, CancellationToken cancellationToken = default)
    {
        var update = Builders<BookDocument>.Update
            .Set(x => x.StockQuantity, stockQuantity)
            .Set(x => x.IsAvailable, stockQuantity > 0)
            .Set(x => x.UpdatedUtc, DateTime.UtcNow);

        var result = await _books.UpdateOneAsync(x => x.Id == bookId, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeactivateAsync(string bookId, CancellationToken cancellationToken = default)
    {
        var update = Builders<BookDocument>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.IsAvailable, false)
            .Set(x => x.UpdatedUtc, DateTime.UtcNow);

        var result = await _books.UpdateOneAsync(x => x.Id == bookId, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    private static Book MapToDomain(BookDocument document)
    {
        return new Book
        {
            BookId = document.Id,
            Title = document.Title,
            ISBN = document.ISBN,
            Author = document.Author,
            Description = document.Description,
            Genre = document.Genre,
            Price = document.Price,
            StockQuantity = document.StockQuantity,
            Language = document.Language,
            PublishedDate = document.PublishedDate,
            IsAvailable = document.IsAvailable,
            IsActive = document.IsActive,
            CreatedUtc = document.CreatedUtc,
            UpdatedUtc = document.UpdatedUtc
        };
    }

    private static BookDocument MapToDocument(Book book)
    {
        return new BookDocument
        {
            Id = book.BookId,
            Title = book.Title,
            ISBN = book.ISBN,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            Language = book.Language,
            PublishedDate = book.PublishedDate,
            IsAvailable = book.IsAvailable,
            IsActive = book.IsActive,
            CreatedUtc = book.CreatedUtc,
            UpdatedUtc = book.UpdatedUtc
        };
    }
}
