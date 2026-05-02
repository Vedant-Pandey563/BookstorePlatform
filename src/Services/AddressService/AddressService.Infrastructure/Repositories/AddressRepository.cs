using AddressService.Application.Contracts;
using AddressService.Domain.Entities;
using AddressService.Infrastructure.Data;
using Dapper;

namespace AddressService.Infrastructure.Repositories;

public sealed class AddressRepository : IAddressRepository
{
    private readonly ISqlConnectionFactory _factory;

    public AddressRepository(ISqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<Address>> GetAllAsync()
    {
        var sql = "SELECT * FROM Addresses";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<Address>(sql)).ToList();
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Addresses WHERE AddressId=@Id";
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Address>(sql, new { Id = id });
    }

    public async Task<List<Address>> GetByUserIdAsync(int userId)
    {
        var sql = "SELECT * FROM Addresses WHERE UserId=@UserId";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<Address>(sql, new { UserId = userId })).ToList();
    }

    public async Task<int> AddAsync(Address address)
    {
        const string sql = @"
            INSERT INTO Addresses
            (Name, MobileNumber, UserAddress, City, State, Type, UserId)
            OUTPUT INSERTED.AddressId
            VALUES
            (@Name, @MobileNumber, @UserAddress, @City, @State, @Type, @UserId);";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, address);
    }

    public async Task<bool> UpdateAsync(Address address)
    {
        const string sql = @"
            UPDATE Addresses
            SET Name = @Name,
                MobileNumber = @MobileNumber,
                UserAddress = @UserAddress,
                City = @City,
                State = @State,
                Type = @Type
            WHERE AddressId = @AddressId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, address) > 0;
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM Addresses WHERE AddressId=@Id", new { Id = id });
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM Addresses WHERE UserId=@UserId", new { UserId = userId });
    }
}
