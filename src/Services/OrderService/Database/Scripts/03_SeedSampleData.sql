USE BookstoreOrdersDb;
GO

INSERT INTO dbo.Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentStatus, IdempotencyKey, CreatedUtc)
VALUES (1, SYSUTCDATETIME(), 49.99, 'Pending', '123 Test St', 'Pending', 'seed-1', SYSUTCDATETIME());

DECLARE @OrderId INT = SCOPE_IDENTITY();

INSERT INTO dbo.OrderItems (OrderId, BookId, BookTitle, UnitPrice, Quantity, LineTotal)
VALUES (@OrderId, 100, 'Sample Book', 49.99, 1, 49.99);
GO
