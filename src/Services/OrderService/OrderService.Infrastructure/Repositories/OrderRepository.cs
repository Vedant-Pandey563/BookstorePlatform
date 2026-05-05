using Dapper;
using OrderService.Application.Abstractions;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public OrderRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Order?> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        try
        {
            // Idempotency check: if the same key was used already, return the existing order.
            var existingId = await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    @"SELECT OrderId
                      FROM dbo.Orders
                      WHERE IdempotencyKey = @IdempotencyKey;",
                    new { order.IdempotencyKey },
                    transaction,
                    cancellationToken: cancellationToken));

            if (existingId.HasValue)
            {
                transaction.Commit();
                return await GetByIdAsync(existingId.Value, cancellationToken);
            }

            const string insertOrderSql = @"
INSERT INTO dbo.Orders
(
    UserId, OrderDate, TotalAmount, Status, ShippingAddress,
    PaymentStatus, IdempotencyKey, CreatedUtc, UpdatedUtc
)
OUTPUT INSERTED.OrderId
VALUES
(
    @UserId, @OrderDate, @TotalAmount, @Status, @ShippingAddress,
    @PaymentStatus, @IdempotencyKey, @CreatedUtc, @UpdatedUtc
);";

            var orderId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertOrderSql, order, transaction, cancellationToken: cancellationToken));

            order.OrderId = orderId;

            const string insertItemSql = @"
INSERT INTO dbo.OrderItems
(
    OrderId, BookId, BookTitle, UnitPrice, Quantity, LineTotal
)
VALUES
(
    @OrderId, @BookId, @BookTitle, @UnitPrice, @Quantity, @LineTotal
);";

            foreach (var item in order.Items)
            {
                item.OrderId = orderId;

                await connection.ExecuteAsync(
                    new CommandDefinition(insertItemSql, item, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();

            return await GetByIdAsync(orderId, cancellationToken);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string orderSql = @"
SELECT
    OrderId,
    UserId,
    OrderDate,
    TotalAmount,
    Status,
    ShippingAddress,
    PaymentStatus,
    IdempotencyKey,
    CreatedUtc,
    UpdatedUtc
FROM dbo.Orders
WHERE OrderId = @OrderId;";

        var order = await connection.QueryFirstOrDefaultAsync<Order>(
            new CommandDefinition(orderSql, new { OrderId = orderId }, cancellationToken: cancellationToken));

        if (order is null)
        {
            return null;
        }

        order.Items = (await GetItemsByOrderIdAsync(orderId, cancellationToken)).ToList();
        return order;
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
SELECT
    OrderId,
    UserId,
    OrderDate,
    TotalAmount,
    Status,
    ShippingAddress,
    PaymentStatus,
    IdempotencyKey,
    CreatedUtc,
    UpdatedUtc
FROM dbo.Orders
WHERE UserId = @UserId
ORDER BY OrderDate DESC;";

        var orders = (await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken))).ToList();

        foreach (var order in orders)
        {
            order.Items = (await GetItemsByOrderIdAsync(order.OrderId, cancellationToken)).ToList();
        }

        return orders;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
SELECT
    OrderId,
    UserId,
    OrderDate,
    TotalAmount,
    Status,
    ShippingAddress,
    PaymentStatus,
    IdempotencyKey,
    CreatedUtc,
    UpdatedUtc
FROM dbo.Orders
ORDER BY OrderDate DESC;";

        var orders = (await connection.QueryAsync<Order>(
            new CommandDefinition(sql, cancellationToken: cancellationToken))).ToList();

        foreach (var order in orders)
        {
            order.Items = (await GetItemsByOrderIdAsync(order.OrderId, cancellationToken)).ToList();
        }

        return orders;
    }

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
SELECT
    OrderId,
    UserId,
    OrderDate,
    TotalAmount,
    Status,
    ShippingAddress,
    PaymentStatus,
    IdempotencyKey,
    CreatedUtc,
    UpdatedUtc
FROM dbo.Orders
WHERE Status = @Status
ORDER BY OrderDate DESC;";

        var orders = (await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { Status = status }, cancellationToken: cancellationToken))).ToList();

        foreach (var order in orders)
        {
            order.Items = (await GetItemsByOrderIdAsync(order.OrderId, cancellationToken)).ToList();
        }

        return orders;
    }

    public async Task<List<OrderItem>> GetItemsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
SELECT
    OrderItemId,
    OrderId,
    BookId,
    BookTitle,
    UnitPrice,
    Quantity,
    LineTotal
FROM dbo.OrderItems
WHERE OrderId = @OrderId
ORDER BY OrderItemId;";

        var items = await connection.QueryAsync<OrderItem>(
            new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: cancellationToken));

        return items.ToList();
    }

    public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
UPDATE dbo.Orders
SET Status = @Status,
    UpdatedUtc = SYSUTCDATETIME(),
    PaymentStatus = CASE
        WHEN @Status = 'Paid' THEN 'Paid'
        WHEN @Status = 'Cancelled' THEN 'Cancelled'
        ELSE PaymentStatus
    END
WHERE OrderId = @OrderId;";

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { OrderId = orderId, Status = status }, cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> CancelAsync(int orderId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
UPDATE dbo.Orders
SET Status = 'Cancelled',
    PaymentStatus = 'Cancelled',
    UpdatedUtc = SYSUTCDATETIME()
WHERE OrderId = @OrderId;";

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: cancellationToken));

        return rows > 0;
    }
}
