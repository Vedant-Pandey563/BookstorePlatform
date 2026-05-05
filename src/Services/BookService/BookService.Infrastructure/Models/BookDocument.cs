using MongoDB.Bson.Serialization.Attributes;

namespace BookService.Infrastructure.Models;

// Mongo-specific document shape.
// This stays in Infrastructure so the Domain layer does not need MongoDB attributes.
[BsonIgnoreExtraElements]
public sealed class BookDocument
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("isbn")]
    public string ISBN { get; set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("genre")]
    public string Genre { get; set; } = string.Empty;

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("stockQuantity")]
    public int StockQuantity { get; set; }

    [BsonElement("language")]
    public string Language { get; set; } = string.Empty;

    [BsonElement("publishedDate")]
    public DateTime PublishedDate { get; set; }

    [BsonElement("isAvailable")]
    public bool IsAvailable { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("createdUtc")]
    public DateTime CreatedUtc { get; set; }

    [BsonElement("updatedUtc")]
    public DateTime? UpdatedUtc { get; set; }
}
