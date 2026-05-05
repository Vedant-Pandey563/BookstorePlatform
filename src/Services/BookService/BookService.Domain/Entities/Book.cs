namespace BookService.Domain.Entities;

// Pure domain entity for the Book Service.
// No MongoDB attributes here so the Domain layer stays clean and independent.
public sealed class Book
{
    // We use string IDs here so MongoDB can store them cleanly without leaking Mongo types into the Domain layer.
    public string BookId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Language { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }

    // Availability means the book can be ordered or shown in active listings.
    public bool IsAvailable { get; set; }

    // Soft-delete / admin control flag.
    public bool IsActive { get; set; } = true;

    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
