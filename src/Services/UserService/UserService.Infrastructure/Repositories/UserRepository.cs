using Dapper;
using UserService.Application.Contracts;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace UserService.Infrastructure.Repositories;

// Dapper repository for all User persistence operations.
// This class contains SQL only and does not use EF Core.
public sealed class UserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, UserName, Email, PhoneNumber, PasswordHash, Role, CreatedUtc
            FROM Users
            ORDER BY UserId;";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var users = await connection.QueryAsync<User>(command);

        return users.ToList();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, UserName, Email, PhoneNumber, PasswordHash, Role, CreatedUtc
            FROM Users
            WHERE UserId = @UserId;";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, new { UserId = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UserId, UserName, Email, PhoneNumber, PasswordHash, Role, CreatedUtc
            FROM Users
            WHERE Email = @Email;";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    public async Task<int> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Users (UserName, Email, PhoneNumber, PasswordHash, Role, CreatedUtc)
            OUTPUT INSERTED.UserId
            VALUES (@UserName, @Email, @PhoneNumber, @PasswordHash, @Role, @CreatedUtc);";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, user, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Users
            SET UserName = @UserName,
                Email = @Email,
                PhoneNumber = @PhoneNumber,
                PasswordHash = @PasswordHash,
                Role = @Role
            WHERE UserId = @UserId;";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, user, cancellationToken: cancellationToken);
        int rowsAffected = await connection.ExecuteAsync(command);

        return rowsAffected > 0;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM Users
            WHERE UserId = @UserId;";

        await using SqlConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, new { UserId = id }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
