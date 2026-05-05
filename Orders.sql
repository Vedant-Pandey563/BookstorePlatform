-- Create the database once in SSMS.
-- Change the name if your solution already has a naming convention.

IF DB_ID('BookstoreOrdersDb') IS NULL
BEGIN
    CREATE DATABASE BookstoreOrdersDb;
END
GO

USE BookstoreOrdersDb;
GO

-- Orders table stores the order header only.
-- No cart/book/user/payment database access here; those are separate bounded contexts.
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
    DROP TABLE dbo.Orders;
GO

CREATE TABLE dbo.Orders
(
    OrderId         INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    OrderDate       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    TotalAmount     DECIMAL(18,2) NOT NULL,
    Status          NVARCHAR(30) NOT NULL,
    ShippingAddress NVARCHAR(300) NOT NULL,
    PaymentStatus   NVARCHAR(30) NOT NULL DEFAULT 'Pending',

    -- Idempotency protects the service from duplicate retries.
    IdempotencyKey  NVARCHAR(100) NOT NULL,

    CreatedUtc      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedUtc      DATETIME2 NULL
);
GO

-- This makes retrying the same checkout request safe.
CREATE UNIQUE INDEX UX_Orders_IdempotencyKey
ON dbo.Orders(IdempotencyKey);
GO

CREATE INDEX IX_Orders_UserId ON dbo.Orders(UserId);
CREATE INDEX IX_Orders_Status ON dbo.Orders(Status);
GO

IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL
    DROP TABLE dbo.OrderItems;
GO

CREATE TABLE dbo.OrderItems
(
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL,
    BookId      INT NOT NULL,
    BookTitle   NVARCHAR(200) NOT NULL,
    UnitPrice   DECIMAL(18,2) NOT NULL,
    Quantity    INT NOT NULL,
    LineTotal   DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
GO

select * from dbo.Orders;
select * from dbo.OrderItems;

