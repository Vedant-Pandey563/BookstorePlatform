using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;

namespace AddressService.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AddressDb")
            ?? throw new Exception("Connection string missing");
    }

    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}
