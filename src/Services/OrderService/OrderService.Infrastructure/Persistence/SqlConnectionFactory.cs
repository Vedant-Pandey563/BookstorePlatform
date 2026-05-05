using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OrderService.Infrastructure.Persistence;

// Central place for creating SQL connections.
// This keeps connection-string usage out of controllers and handlers.
public sealed class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing connection string: ConnectionStrings:DefaultConnection");
        }

        return new SqlConnection(connectionString);
    }
}
