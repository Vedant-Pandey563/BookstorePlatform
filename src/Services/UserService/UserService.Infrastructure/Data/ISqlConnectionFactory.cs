using Microsoft.Data.SqlClient;

namespace UserService.Infrastructure.Data;

// Creates SQL Server connections for Dapper.
// Keeping this behind an interface makes the repository easier to test later if needed.
public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
