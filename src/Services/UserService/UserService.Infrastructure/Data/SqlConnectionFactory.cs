using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UserService.Infrastructure.Data;

// Reads the connection string from configuration and creates SQL Server connections.
// This is the single place where the database connection string is resolved.
public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("UserDb")
            ?? throw new InvalidOperationException("Connection string 'UserDb' was not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
