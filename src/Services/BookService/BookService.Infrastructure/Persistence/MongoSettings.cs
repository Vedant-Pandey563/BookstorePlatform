namespace BookService.Infrastructure.Persistence;

// Mongo settings are read from appsettings.json.
// Compass is only the UI; the service connects to the local MongoDB server.
public sealed class MongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "BookstoreCatalogDb";
    public string BooksCollectionName { get; set; } = "books";
}
